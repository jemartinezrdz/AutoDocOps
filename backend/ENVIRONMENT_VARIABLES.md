# Environment Variables Configuration

## Required Environment Variables

The following environment variables must be configured for the application to work properly:

### JWT Configuration
- `JWT_KEY`: Secret key for JWT token generation (minimum 32 characters)
- `JWT_ISSUER`: JWT token issuer (default: "AutoDocOps")
- `JWT_AUDIENCE`: JWT token audience (default: "AutoDocOps-Users")
- `JWT_EXPIRY_HOURS`: JWT token expiry in hours (default: 24)

### Database Configuration
- `CONNECTION_STRINGS__DEFAULTCONNECTION`: Database connection string
- `SUPABASE__URL`: Supabase project URL
- `SUPABASE__KEY`: Supabase anon key
- `SUPABASE__SERVICEKEY`: Supabase service role key
- `SUPABASE__JWTSECRET`: Supabase JWT secret

### OpenAI Configuration
- `OPENAI__APIKEY`: OpenAI API key
- `OPENAI__MODEL`: OpenAI model to use (default: "gpt-4o-mini")
- `OPENAI__EMBEDDINGMODEL`: OpenAI embedding model (default: "text-embedding-3-small")
- `OPENAI__MAXTOKENS`: Maximum tokens for OpenAI requests (default: 4000)
- `OPENAI__TEMPERATURE`: Temperature for OpenAI requests (default: 0.1)

### CORS Configuration
- `CORS__ALLOWEDORIGINS__0`: First allowed origin
- `CORS__ALLOWEDORIGINS__1`: Second allowed origin (add more as needed)

## Example .env file

Create a `.env` file in the backend root directory:

```env
# JWT Configuration
JWT_KEY=your-super-secret-jwt-key-with-at-least-32-characters
JWT_ISSUER=AutoDocOps
JWT_AUDIENCE=AutoDocOps-Users
JWT_EXPIRY_HOURS=24

# Database Configuration
CONNECTION_STRINGS__DEFAULTCONNECTION=Host=localhost;Database=autodocops;Username=postgres;Password=your-password

# Supabase Configuration
SUPABASE__URL=https://your-project.supabase.co
SUPABASE__KEY=your-anon-key
SUPABASE__SERVICEKEY=your-service-role-key
SUPABASE__JWTSECRET=your-jwt-secret

# OpenAI Configuration
OPENAI__APIKEY=your-openai-api-key
OPENAI__MODEL=gpt-4o-mini
OPENAI__EMBEDDINGMODEL=text-embedding-3-small
OPENAI__MAXTOKENS=4000
OPENAI__TEMPERATURE=0.1

# CORS Configuration
CORS__ALLOWEDORIGINS__0=http://localhost:8081
CORS__ALLOWEDORIGINS__1=https://your-frontend-domain.com
```

## Docker Configuration

For Docker deployments, these environment variables can be passed using:

1. Docker Compose `.env` file
2. Docker run `-e` parameters
3. Kubernetes ConfigMaps and Secrets

## Security Notes

- Never commit the `.env` file to version control
- Use strong, randomly generated keys for JWT secrets
- Rotate secrets regularly
- Use different secrets for different environments
- Store production secrets in secure key management systems (Azure Key Vault, AWS Secrets Manager, etc.)