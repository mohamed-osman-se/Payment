<img width="64" height="64" alt="image" src="https://github.com/user-attachments/assets/261af8db-c9a5-4b49-9c5c-5b003e130ce1" />

# ShopLite – Online Store App  
Welcome to ShopLite — a production-ready online store web app that lets admins manage products, users create accounts, shop, and securely complete payments using Stripe integration.

I built this project to demonstrate backend engineering skills in real-world e-commerce scenarios, including authentication, payment processing, state management, CI/CD pipelines, and containerized deployments.  

🔗 **Live Demo (HTTPS Enabled):**  
https://payment-production-48f5.up.railway.app/
https://payment-dloj91ec.b4a.run/
---

## ✨ Why This Project Stands Out  

✅ **Stripe Integration** – Secure online payments with webhook confirmation  
✅ **State Management** – Handles `Pending`, `Awaiting Payment`, `Cancelled`, and `Completed` orders with non-repudiation (no duplicate charges on refresh or network loss)  
✅ **Full E-Commerce Flow** – Admin manages products, users register, and checkout with Stripe  
✅ **Dockerized Deployment** – Consistent experience across local, staging, and production environments  
✅ **Unit Tests** – Ensure reliability of core business logic  

---

## 📦 CI/CD Workflow  

On each push to `main`:  

1. Run unit tests  
2. Build Docker image  
3. Push image to Docker Hub  
4. Deploy the updated app automatically to Railway  

---

## 📸 Screenshots  
<img width="1299" height="703" alt="image" src="https://github.com/user-attachments/assets/e03d7e36-d7f6-4828-bcd6-afdc642f5e8a" />  
<img width="1299" height="703" alt="image" src="https://github.com/user-attachments/assets/9fe28c27-730a-4f7c-8520-db7b03db3c21" />  
<img width="1299" height="703" alt="image" src="https://github.com/user-attachments/assets/2fecd417-30c5-48cc-8708-57d5b91f9b5d" />  

---

## 🛠️ Technologies  

| Area            | Technology |
|-----------------|------------|
| Language        | C# (.NET 9 SDK) |
| Framework       | ASP.NET Core MVC |
| Authentication  | Cookie-based authentication |
| Database        | EF Core (postgresql  database) |
| Payment         | Stripe (Checkout + Webhooks) |
| CI/CD           | GitHub Actions |
| Hosting         | Railway |
| Containerization| Docker + Docker Hub |
| Testing         | xUnit |

---

## 🚀 Getting Started  

### Prerequisites  
- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0)  
- [Docker](https://www.docker.com/)  
- Stripe API keys (test or live mode)  

---

### Run Locally  

```bash
# Clone the repository
git clone https://github.com/mohamed-osman-se/Payment.git
cd ~/Payment/src/Payment

# Restore and run
dotnet restore
dotnet run
```

### Run with Docker

```bash
git clone https://github.com/mohamed-osman-se/Payment.git
cd ~/Payment/src/Payment

docker build -t payment .
docker run -p 8080:80 payment

