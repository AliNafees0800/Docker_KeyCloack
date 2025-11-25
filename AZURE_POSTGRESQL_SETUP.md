# Azure PostgreSQL Server Setup Guide

## Prerequisites

Open the Azure Portal from your Microsoft Account:

**Portal URL:** https://portal.azure.com/?l=en.en-us#browse/Microsoft.DBforPostgreSQL%2Fservers

---

## Step 1: Create New Server

Click on the **Create** button to start the setup process.

---

## Step 2: Basic Configuration

### Project Details
- Select **Subscription**
- Add a **separate resource group** or select an **existing** one

### Server Details
- Add **Server name**
- Select **Region**
- Select **PostgreSQL version** (18 is current)
- Select **Workload type** as **dev** or **prod**

### Virtual Machine Selection
- Select appropriate VM
  - For **dev**: Choose **Standard_B1MS** (1 CPU, 2GB RAM)
- VM also provides other options:
  - Storage options
  - Backups
  - IOPS selection
  - Data redundancy

### Additional Basic Settings
- **Availability Zone**: Keep **No preference**
- **Zonal resiliency**: Keep **disabled**
- **Authentication method**: Select **PostgreSQL authentication only**

### Administrator Credentials
- Add **PostgreSQL user login** as **Administrator login**
- Add **password** and **confirm password** (required to connect to remote PostgreSQL)

---

## Step 3: Networking Configuration

### Connectivity Method
- Keep **Public** as the connectivity method

### Public Access Settings
- ✅ Check this checkbox: **Allow public access to this resource through the internet using a public IP address**
- ✅ Check this checkbox: **Allow public access from any Azure service within Azure to this server**

### Firewall Rules
- In **Firewall rules**, click **+ Add**
- Add rule: **0.0.0.0 - 255.255.255.255**

---

## Step 4: Security Configuration

### Data Encryption
- In **Security** tab, under **Data encryption key**, select **Service-managed key**

### Tags
- Add appropriate **tag names** as needed

---

## Step 5: Review and Create

### Final Steps
- Navigate to the **Review and create** tab
- Review all settings
- Click **Create** after reviewing

---

## Step 6: Connect and Create Database

### After Server Creation
- Once the VM/server is created, **open the resource** in Azure Portal
- Click on **Command Connect from VS Code**

### Connect via VS Code
- VS Code will open automatically
- The connection will provide the **host** information
- Use the **username** and **password** that were added in **Step 2** (Administrator login credentials)
- **Login** to the PostgreSQL server

### Create Database
- After successful login, add a database with the name **keycloak**
- That's it! The database is now ready to use

---

## Notes

- Ensure you have the necessary permissions in your Azure subscription
- Save the administrator login credentials securely
- The firewall rule (0.0.0.0 - 255.255.255.255) allows access from any IP address - consider restricting this for production environments

