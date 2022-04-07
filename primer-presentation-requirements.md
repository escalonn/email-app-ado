p1
    ERS, java, spring boot, mvc, pgsql (gcp), docker compose (2 apis)
p2
    Spring Boot, Spring Data, Spring MVC Java Maven Terraform pgsql (gcp) Prometheus/Grafana/Loki, GKE, Jenkins (non-container) Docker (~2 images)
p3
    deployed pre-created
    +helm, amazon s3

----
requirements for next week friday, individual presentation

azure boards: pick a work item process, set up 10+ work items
    to essentially document the current state of the development
azure repos: the code should be in 1 or more azure git repos.
    define a branching model for the project, document it in the README/CONTRIBUTING.md, and set up appropriate branch policies.
azure pipelines
    CI: build, test, static analysis, reporting
        builds for pull requests when reasonable
        triggers, conditions, etc. should reflect the branching model
    CD: deploy to Azure Kubernetes Service (optionally with helm)
        manage azure resources with terraform (optionally: as a deployment step, not manually)
    use azure pipelines features idiomatically
optional: some thought into notifications (from any/all parts of the project)
optional: break the pipeline(s) up into more reusable pieces using templates
