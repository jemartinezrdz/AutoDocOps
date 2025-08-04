# ADR-002: API Versioning Strategy

**Status:** Accepted
**Date:** 2025-01-08
**Deciders:** AutoDocOps Team

## Context

AutoDocOps API needs a versioning strategy that allows for backward compatibility while enabling evolution of the API. We need to support multiple clients (web, mobile, CLI tools) that may update at different rates.

## Decision

We will implement API versioning using:

1. **Header-based versioning** (primary): `api-version: 1.0`
2. **Query parameter versioning** (fallback): `?version=1.0`
3. **Semantic Versioning (SemVer)** for version numbers

### Versioning Rules:
- **Major version** (X.0): Breaking changes
- **Minor version** (X.Y): New features, backward compatible
- **Patch version** (X.Y.Z): Bug fixes, backward compatible

### URL Structure:
- `/api/v{major}/resource` (e.g., `/api/v1/documentation`)
- Version in header takes precedence over URL

## Consequences

### Positive:
- Clear versioning strategy
- Backward compatibility maintained
- Flexibility for clients to upgrade gradually
- Industry standard approach

### Negative:
- Additional complexity in routing
- Need to maintain multiple versions
- Documentation overhead

## Implementation Details

- Use NSwag for OpenAPI documentation generation
- Implement custom API versioning middleware
- Default to latest stable version when no version specified
- Deprecation notices for old versions with 6-month sunset period