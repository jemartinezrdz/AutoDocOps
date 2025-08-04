terraform {
  required_version = ">= 1.6"
  required_providers {
    fly = {
      source  = "fly-apps/fly"
      version = "~> 0.1.0"
    }
    cloudflare = {
      source  = "cloudflare/cloudflare"
      version = "~> 4.0"
    }
  }
}

# Variables
variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be one of: dev, staging, prod."
  }
}

variable "app_name" {
  description = "Application name"
  type        = string
  default     = "autodocops"
}

variable "cloudflare_zone_id" {
  description = "Cloudflare Zone ID"
  type        = string
  sensitive   = true
}

variable "domain_name" {
  description = "Domain name for the application"
  type        = string
}

variable "supabase_url" {
  description = "Supabase project URL"
  type        = string
  sensitive   = true
}

variable "supabase_anon_key" {
  description = "Supabase anonymous key"
  type        = string
  sensitive   = true
}

variable "supabase_service_role_key" {
  description = "Supabase service role key"
  type        = string
  sensitive   = true
}

# Locals
locals {
  app_full_name = "${var.app_name}-${var.environment}"
  common_tags = {
    Environment = var.environment
    Project     = var.app_name
    ManagedBy   = "terraform"
  }
}

# Fly.io Application
resource "fly_app" "autodocops_api" {
  name = local.app_full_name
  org  = "autodocops"
}

# Fly.io Machine Configuration
resource "fly_machine" "autodocops_api" {
  app    = fly_app.autodocops_api.name
  region = var.environment == "prod" ? "iad" : "sjc" # US East for prod, US West for dev/staging
  name   = "${local.app_full_name}-machine"

  image = "registry.fly.io/${local.app_full_name}:latest"

  services = [
    {
      ports = [
        {
          port     = 443
          handlers = ["tls", "http"]
        },
        {
          port     = 80
          handlers = ["http"]
        }
      ]
      protocol      = "tcp"
      internal_port = 8080
    }
  ]

  env = {
    ASPNETCORE_ENVIRONMENT = var.environment == "prod" ? "Production" : title(var.environment)
    ASPNETCORE_URLS        = "http://+:8080"
    Supabase__Url          = var.supabase_url
  }

  # Resource allocation based on environment
  cpus     = var.environment == "prod" ? 2 : 1
  memory   = var.environment == "prod" ? 2048 : 512
  cputype  = "shared"
  
  # Auto-scaling configuration
  restart_policy = "always"
  
  # Health check
  checks = [
    {
      grace_period = "10s"
      interval     = "30s"
      timeout      = "5s"
      method       = "GET"
      path         = "/health"
      protocol     = "http"
      port         = 8080
    }
  ]
}

# Fly.io Secrets
resource "fly_secret" "supabase_anon_key" {
  app   = fly_app.autodocops_api.name
  name  = "Supabase__AnonKey"
  value = var.supabase_anon_key
}

resource "fly_secret" "supabase_service_role_key" {
  app   = fly_app.autodocops_api.name
  name  = "Supabase__ServiceRoleKey"  
  value = var.supabase_service_role_key
}

# Fly.io Volume for persistent storage (if needed)
resource "fly_volume" "autodocops_data" {
  count     = var.environment == "prod" ? 1 : 0
  app       = fly_app.autodocops_api.name
  name      = "${local.app_full_name}-data"
  size      = 10
  region    = "iad"
  encrypted = true
}

# Cloudflare DNS Records
resource "cloudflare_record" "autodocops_api" {
  zone_id = var.cloudflare_zone_id
  name    = var.environment == "prod" ? "api" : "api-${var.environment}"
  value   = "${local.app_full_name}.fly.dev"
  type    = "CNAME"
  ttl     = 300
  proxied = true

  comment = "AutoDocOps API ${var.environment} environment"
}

# Cloudflare Page Rules for caching
resource "cloudflare_page_rule" "autodocops_api_cache" {
  zone_id  = var.cloudflare_zone_id
  target   = "${cloudflare_record.autodocops_api.hostname}/health"
  priority = 1

  actions {
    cache_level = "cache_everything"
    edge_cache_ttl = 300
  }
}

# Cloudflare Security Rules
resource "cloudflare_rate_limit" "autodocops_api" {
  zone_id   = var.cloudflare_zone_id
  threshold = 100
  period    = 60
  
  match {
    request {
      url_pattern = "${cloudflare_record.autodocops_api.hostname}/*"
      schemes     = ["HTTP", "HTTPS"]
      methods     = ["GET", "POST", "PUT", "DELETE", "PATCH"]
    }
  }
  
  action {
    mode    = "ban"
    timeout = 300
    
    response {
      content_type = "application/json"
      body         = jsonencode({
        error = "Rate limit exceeded"
        code  = 429
      })
    }
  }
  
  correlate {
    by = "nat"
  }
  
  disabled    = false
  description = "Rate limiting for AutoDocOps API ${var.environment}"
}

# Cloudflare Firewall Rules
resource "cloudflare_filter" "block_bad_bots" {
  zone_id     = var.cloudflare_zone_id
  description = "Block known bad bots for AutoDocOps ${var.environment}"
  expression  = "(cf.client.bot) and not (cf.verified_bot_category in {\"Search Engine\" \"Security\"})"
}

resource "cloudflare_firewall_rule" "block_bad_bots" {
  zone_id     = var.cloudflare_zone_id
  description = "Block bad bots rule for AutoDocOps ${var.environment}"
  filter_id   = cloudflare_filter.block_bad_bots.id
  action      = "block"
  priority    = 1000
}

# Outputs
output "app_url" {
  description = "Application URL"
  value       = "https://${cloudflare_record.autodocops_api.hostname}"
}

output "fly_app_name" {
  description = "Fly.io application name"
  value       = fly_app.autodocops_api.name
}

output "cloudflare_record_name" {
  description = "Cloudflare record name"
  value       = cloudflare_record.autodocops_api.hostname
}