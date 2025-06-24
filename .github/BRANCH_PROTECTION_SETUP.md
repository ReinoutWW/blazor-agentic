# Branch Protection Setup Guide

This guide explains how to configure GitHub branch protection rules to enforce the strict CI/CD pipeline for HealthVoice.

## 🛡️ Required Branch Protection Settings

### Step 1: Navigate to Branch Protection Rules

1. Go to your repository on GitHub
2. Click **Settings** → **Branches**
3. Click **Add rule** or edit existing rule for `master`/`main`

### Step 2: Configure Protection Rules

#### ✅ Basic Protection

- **Branch name pattern**: `master` (or `main`)
- ☑️ **Require a pull request before merging**
  - ☑️ **Require approvals**: `2` (recommended for production)
  - ☑️ **Dismiss stale PR approvals when new commits are pushed**
  - ☑️ **Require review from code owners** (if using CODEOWNERS)

#### ✅ Status Checks (Critical)

- ☑️ **Require status checks to pass before merging**
- ☑️ **Require branches to be up to date before merging**

**Required Status Checks** (select all that apply):
- `CI (strict) / lint-and-format`
- `CI (strict) / test`
- `CI (strict) / security`
- `CI (strict) / docker-build (api)`
- `CI (strict) / docker-build (blazor)`
- `CI (strict) / integration`
- `CI (strict) / quality-gate`
- `Security Scan / codeql`
- `Security Scan / dependency-scan`
- `Security Scan / container-security (api)`
- `Security Scan / container-security (blazor)`
- `Security Scan / secret-scan`

#### ✅ Additional Security

- ☑️ **Require conversation resolution before merging**
- ☑️ **Require signed commits** (recommended)
- ☑️ **Require linear history** (prevents merge commits)
- ☑️ **Include administrators** (applies rules to admins too)

### Step 3: Enable Merge Queue (Recommended)

1. Go to **Settings** → **Pull request merges**
2. Scroll to **Merge queue**
3. ☑️ **Enable merge queue**
4. Configure settings:
   - **Merge method**: `Squash and merge` (recommended)
   - **Maximum queue size**: `5` (adjust based on team size)
   - **Minimum queue time**: `0` minutes
   - **Maximum queue time**: `60` minutes

## 🔧 Advanced Configuration

### CODEOWNERS File

Create `.github/CODEOWNERS` to require specific reviewers:

```
# Global owners
* @team-leads @security-team

# API layer requires backend team review
HealthVoice.Api/ @backend-team @api-team

# Infrastructure changes require DevOps review
docker-compose*.yml @devops-team
.github/workflows/ @devops-team @security-team
Dockerfile* @devops-team

# Security-sensitive files require security team review
SECURITY.md @security-team
.github/workflows/security.yml @security-team
```

### Rulesets (Organization-wide)

For consistent rules across all repositories:

1. Go to **Organization Settings** → **Repository rulesets**
2. Create a new ruleset
3. Apply to all repositories or specific patterns
4. Configure the same rules as above

## 🚨 Emergency Procedures

### Temporary Rule Bypass

For critical hotfixes, administrators can:

1. Temporarily disable branch protection
2. Apply emergency fix
3. Re-enable protection immediately
4. Create post-incident review

### Rollback Process

If a deployment needs rollback:

1. Use the **Manual Rollback** workflow
2. Go to **Actions** → **CD (Deploy)**
3. Click **Run workflow**
4. Select environment and rollback option

## 📊 Quality Gates Summary

The strict pipeline enforces these gates:

| Gate | Threshold | Blocking |
|------|-----------|----------|
| **Code Formatting** | 0 warnings | ✅ |
| **Build** | Must succeed | ✅ |
| **Unit Tests** | 100% pass | ✅ |
| **Code Coverage** | ≥ 85% | ✅ |
| **Security Scan** | 0 critical/high | ✅ |
| **Dependency Scan** | 0 vulnerable packages | ✅ |
| **Container Scan** | 0 critical vulnerabilities | ✅ |
| **Secret Detection** | 0 secrets found | ✅ |
| **Integration Tests** | All endpoints healthy | ✅ |

## 🔄 Workflow Integration

### Pull Request Flow

1. Developer creates PR
2. All CI checks run automatically
3. Security scans execute
4. Code review required (2 approvals)
5. All status checks must be green
6. PR can be added to merge queue
7. Final verification in queue
8. Automatic merge on success

### Deployment Flow

1. Merge to master triggers CD pipeline
2. Docker images built and pushed
3. Automatic deployment to development
4. Manual approval for staging
5. Manual approval for production
6. Rollback available if needed

## 🛠️ Troubleshooting

### Common Issues

**Status check not appearing:**
- Ensure workflow has run at least once
- Check workflow name matches exactly
- Verify branch name in workflow trigger

**Merge queue stuck:**
- Check if any required checks are failing
- Verify queue configuration
- Look for conflicting branch rules

**Emergency bypass needed:**
- Only administrators can disable protection
- Document reason for bypass
- Re-enable protection immediately after fix

### Support

For issues with branch protection setup:

1. Check GitHub documentation
2. Review workflow logs
3. Contact DevOps team
4. Create support ticket if needed

---

**Last Updated**: 2025-01-24  
**Review Schedule**: Monthly 