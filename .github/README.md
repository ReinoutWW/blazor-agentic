# HealthVoice GitHub Actions Workflows

This directory contains the CI/CD pipeline workflows for HealthVoice, implementing a strict quality gate system that ensures code quality, security, and reliability.

## 📋 Workflow Overview

### 🔄 CI (Continuous Integration) - `ci.yml`

**Triggers**: Pull requests to `master`/`main`  
**Purpose**: Comprehensive quality checks before code can be merged

**Jobs**:
- **Lint & Format**: Code formatting and static analysis
- **Test Suite**: Unit tests with 85% coverage requirement
- **Security Scan**: Vulnerability detection in dependencies
- **Docker Build**: Multi-platform container builds
- **Integration Tests**: Full-stack testing with Docker Compose
- **Quality Gate**: Final validation of all checks

**Quality Thresholds**:
- ✅ 0 linting warnings
- ✅ 100% test pass rate
- ✅ ≥85% code coverage
- ✅ 0 vulnerable dependencies
- ✅ Successful Docker builds
- ✅ All integration tests pass

### 🚀 CD (Continuous Deployment) - `cd.yml`

**Triggers**: Pushes to `master`/`main` or manual dispatch  
**Purpose**: Automated deployment pipeline with environment promotion

**Environments**:
- **Development**: Auto-deploy on merge
- **Staging**: Manual approval required
- **Production**: Manual approval + extra safety checks

**Features**:
- Multi-platform Docker image builds (AMD64/ARM64)
- GitHub Container Registry publishing
- Environment-specific configurations
- Smoke tests and health checks
- Emergency rollback capabilities

### 🛡️ Security Scanning - `security.yml`

**Triggers**: PRs, pushes, daily schedule (2 AM UTC)  
**Purpose**: Comprehensive security analysis

**Security Checks**:
- **CodeQL**: Static application security testing (SAST)
- **Dependency Scan**: Vulnerable package detection
- **Container Security**: Docker image vulnerability scanning
- **Secret Scanning**: Credential detection in code
- **IaC Security**: Infrastructure as Code security validation
- **Compliance**: Security policy adherence

## 🔧 Workflow Configuration

### Environment Variables

```yaml
DOTNET_VERSION: '8.0.x'
REGISTRY: ghcr.io
IMAGE_NAME: ${{ github.repository }}
```

### Required Secrets

Set these in **Settings** → **Secrets and variables** → **Actions**:

- `CODECOV_TOKEN`: For code coverage reporting
- `GITLEAKS_LICENSE`: For enhanced secret scanning
- `SLACK_WEBHOOK`: For deployment notifications (optional)

### Required Environments

Configure in **Settings** → **Environments**:

- `development`: Auto-deploy environment
- `staging`: Manual approval required
- `production`: Manual approval + protection rules
- `*-rollback`: Emergency rollback environments

## 📊 Quality Gates

### CI Pipeline Gates

| Check | Requirement | Failure Action |
|-------|-------------|----------------|
| Code Formatting | `dotnet format --verify-no-changes` | ❌ Block merge |
| Build | `dotnet build -warnaserror` | ❌ Block merge |
| Unit Tests | 100% pass rate | ❌ Block merge |
| Code Coverage | ≥85% line coverage | ❌ Block merge |
| Security Scan | 0 vulnerabilities | ❌ Block merge |
| Docker Build | Successful build | ❌ Block merge |
| Integration Tests | All health checks pass | ❌ Block merge |

### Security Pipeline Gates

| Check | Requirement | Failure Action |
|-------|-------------|----------------|
| CodeQL | 0 critical/high findings | ❌ Block merge |
| Dependencies | 0 vulnerable packages | ❌ Block merge |
| Container Scan | 0 critical vulnerabilities | ⚠️ Alert only |
| Secret Detection | 0 secrets found | ❌ Block merge |
| Compliance | All policies met | ❌ Block merge |

## 🚀 Usage Guide

### For Developers

1. **Create feature branch**: `git checkout -b feature/my-feature`
2. **Make changes** following HealthVoice coding standards
3. **Commit changes**: Use conventional commit messages
4. **Push branch**: `git push origin feature/my-feature`
5. **Create PR**: All CI checks run automatically
6. **Address failures**: Fix any red status checks
7. **Get approvals**: Minimum 2 code reviews required
8. **Merge**: Use "Add to merge queue" when available

### For Deployments

#### Automatic (Development)
- Merging to `master` automatically deploys to development
- Smoke tests run post-deployment
- Notifications sent to team channels

#### Manual (Staging/Production)
1. Go to **Actions** → **CD (Deploy)**
2. Click **Run workflow**
3. Select target environment
4. Provide approval in environment protection
5. Monitor deployment progress
6. Verify health checks post-deployment

#### Emergency Rollback
1. Go to **Actions** → **CD (Deploy)**
2. Click **Run workflow**
3. Select environment and rollback option
4. Confirm rollback in environment protection
5. Verify rollback success

## 🔍 Monitoring & Debugging

### Workflow Status

Check workflow status at:
- **Actions tab**: Overview of all workflow runs
- **PR checks**: Status checks on pull requests
- **Branch protection**: Required checks configuration

### Common Issues

**❌ Build Failures**
```bash
# Check locally before pushing
dotnet restore
dotnet build --configuration Release -warnaserror
dotnet test
```

**❌ Coverage Below Threshold**
```bash
# Run coverage locally
dotnet test --collect:"XPlat Code Coverage"
# Add tests for uncovered code paths
```

**❌ Security Scan Failures**
```bash
# Check for vulnerabilities
dotnet list package --vulnerable --include-transitive
# Update vulnerable packages
dotnet add package <PackageName> --version <SafeVersion>
```

**❌ Docker Build Issues**
```bash
# Test Docker build locally
docker build -f HealthVoice.Api/Dockerfile .
docker build -f HealthVoice.Blazor/Dockerfile .
```

### Logs and Artifacts

- **Test Results**: Uploaded as artifacts for failed runs
- **Coverage Reports**: Available in Codecov dashboard
- **Security Reports**: SARIF files uploaded to Security tab
- **Build Logs**: Available in workflow run details

## 📈 Metrics & Reporting

### CI/CD Metrics

We track the following metrics:

- **Build Success Rate**: Target >95%
- **Average Build Time**: Target <15 minutes
- **Test Coverage**: Target ≥85%
- **Security Findings**: Target 0 critical/high
- **Deployment Frequency**: Daily to development
- **Lead Time**: Code to production <1 day
- **MTTR**: Mean time to recovery <2 hours

### Dashboards

- **GitHub Insights**: Repository analytics
- **Codecov**: Code coverage trends
- **Security Tab**: Security findings overview
- **Actions**: Workflow success rates

## 🔄 Maintenance

### Regular Tasks

- **Weekly**: Review failed workflows and trends
- **Monthly**: Update workflow dependencies
- **Quarterly**: Review and update quality thresholds
- **Annually**: Security workflow audit

### Workflow Updates

When updating workflows:

1. Test changes in feature branch
2. Validate with sample PR
3. Get DevOps team review
4. Document changes in this README
5. Update branch protection rules if needed

## 📚 References

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [HealthVoice Testing Strategy](../docs/testing-strategy.md)
- [Branch Protection Setup](./BRANCH_PROTECTION_SETUP.md)
- [Security Policy](../SECURITY.md)

---

**Last Updated**: 2025-01-24  
**Maintained by**: DevOps Team  
**Next Review**: 2025-04-24 