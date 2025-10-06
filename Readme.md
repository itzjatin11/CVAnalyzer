🧠 CV Analyzer Dashboard
📄 Overview

The CV Analyzer Dashboard is an intelligent ASP.NET Core MVC web application that automates the process of analyzing and organizing student or candidate CVs. It allows users to upload individual or multiple resumes, automatically extract key information using NLP (Natural Language Processing), and generate insightful reports and clusters based on skills and experience overlap.

This project is especially useful for universities, recruiters, and training institutions that need to efficiently group and analyze large batches of CVs.

🚀 Key Features

✅ Single & Bulk CV Uploads
Upload one or multiple CVs (PDF/DOCX) at a time.

✅ Automated Data Extraction (via Affinda API)
Extracts important fields such as:

Student ID

Name

Skills

Work Experience

✅ Data Storage
Stores extracted data in a structured format using a local or SQL Server database.

✅ Excel Report Generation
Generate downloadable Excel reports for all extracted candidates’ data.

✅ Skill-Based Grouping
Uses NLP and similarity detection to identify related or overlapping skills and group candidates accordingly.

✅ Cluster Visualization
View groups of students/candidates with shared skills or similar experience in the dashboard.

✅ Stats Dashboard (optional)
Visualize overall skill distribution, experience levels, and keyword frequency summaries.

🧩 Tech Stack
Category	Technology
Frontend	Razor Views (HTML, CSS, Bootstrap)
Backend	ASP.NET Core MVC (C#)
Database	SQL Server / EF Core
NLP API	Affinda Resume Parsing API

Reports	EPPlus / ClosedXML for Excel Export
Architecture	MVC pattern + optional 3-tier structure
⚙️ Project Structure
CVAnalyzer/
├── Controllers/
│   ├── HomeController.cs
│   ├── UploadController.cs
│   ├── StatsController.cs
│   └── ReportsController.cs
│
├── Models/
│   ├── Candidate.cs
│   ├── SkillGroup.cs
│   └── Experience.cs
│
├── Services/
│   ├── AffindaService.cs
│   ├── ExcelReportService.cs
│   └── GroupingService.cs
│
├── Views/
│   ├── Home/
│   ├── Upload/
│   ├── Reports/
│   └── Stats/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── uploads/
│
├── appsettings.json
└── CVAnalyzer.csproj

🔑 Affinda API Integration

To use the Affinda Resume Parsing API, you’ll need an API key.

Create an account on Affinda
.

Get your API key from the developer dashboard.

Add it to your appsettings.json:

"AffindaSettings": {
  "ApiKey": "YOUR_API_KEY_HERE"
}


The AffindaService handles all HTTP requests to the Affinda API for parsing resumes and extracting structured data.

🧮 Skill Grouping Logic

The grouping module uses string similarity and semantic matching to identify related skills (e.g., “C#” ≈ “.NET”, “Python” ≈ “Machine Learning”) using Affinda’s extracted keywords and internal NLP models.
Candidates with overlapping skills are grouped and displayed together, with the option to export group data as Excel reports.

📊 Excel Report

The generated report includes columns such as:

Student ID	Name	Skills	Experience	Cluster

Each record corresponds to a parsed and processed CV entry.

🧰 Setup Instructions

Clone the Repository:

git clone https://github.com/yourusername/CVAnalyzer.git
cd CVAnalyzer


Configure Database:

Update your appsettings.json connection string:

"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=CVAnalyzerDB;Trusted_Connection=True;"
}


Run EF Core migrations (if applicable):

dotnet ef database update


Add API Key:
Insert your Affinda API key under AffindaSettings.

Run the Application:

dotnet run


Visit http://localhost:5000
 in your browser.

📸 Screenshots (Optional)

You can include screenshots such as:

Dashboard View

Upload Page

Cluster Groups

Excel Report Preview

🧑‍💻 Future Improvements

Add authentication for admin/user roles

Integrate more advanced NLP models for skill similarity

Include PDF preview and editing options

Export grouped reports with visual analytics

🏆 Author

Jatin Singh Taadiyal
🎓 Bachelor of Information Technology — AIS, Auckland
💼 Passionate about full-stack development, AI, and automation.
📧 jatintaadiyal@gmail.com | 🌐 https://www.linkedin.com/in/jatinsinghtaadiyal/
