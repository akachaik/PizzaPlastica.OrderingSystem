# Building cloud native applications with Microsoft Orleans

https://blog.codiceplastico.com/building-cloud-native-applications-with-microsoft-orleans/

We need an application that scales, probably we could deploy the application on premise initially, but after a few months we should migrate the application to the cloud. We want to keep the application as easy as possible, with minimal dependencies on the infrastructure (our dream is to manage only the application and the database), but we need a cache because our tables are stateful entities. The migration to the cloud should be painless, theoretically moving only the data to the cloud database and deploying the application somewhere. And we need a scheduled task to close the table ordination.
With these requirements and these bullet points (cached and stateful entities, cloud-native application, at least a scheduled job) Microsoft Orleans could be the right choice
