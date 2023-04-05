# JobBoards - PJLI Connex
A user-friendly web application that enhances the job search experience for both job seekers and hiring managers!

### Technologies:
- C#, .NET 7
- .NET Core MVC Web Application
- .NET Core Web API
- SQL Server 2022
- Docker and Kubernetes

# Members
- Joagne Ann F. Fernando
- Prince Joshue M. Guintu
- Raphael Isiah L. Ramirez

# Setup
Firstly, make sure you have docker and kubernetes installed and running on your computer. Once done, follow the step below.
1. Clone the repository.
```bash
git clone https://github.com/ramirezraph/JobBoards.git
```

2. Go to the project's root directory.
```bash
cd JobBoards
```

3. Go to the `k8` directory and apply the kubernetes file.
```
cd k8s
kubectl apply -f .\deployment.yaml
```

4. Check if all the services is running with this command:
```bash
kubectl get all -n jobboard
```

5. To access the web applications, you can use the `kubectl port-forward` command to forward the webapp or webapi service port to your local machine.

To port-forward the web application:
```bash
kubectl port-forward svc/jobboardswebapp 5001:5001 -n jobboard
```

To port-forward the web api:
```bash
kubectl port-forward svc/jobboardsapi 6001:6001 -n jobboard
```

Then, you can now access the web application or web api in your browser by going to `http://localhost:<port>`

# Screenshots
