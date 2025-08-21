# 🏏 CrickSim Pro

**CrickSim Pro** is an advanced, real-time cricket tactics simulator that empowers fans, aspiring coaches, and analysts to design, simulate, and test strategic decisions in live-like match scenarios.

---

## 🎯 Project Objective

Build a full-stack web application that allows users to:
- Simulate overs and innings based on match conditions.
- Experiment with field placements, bowling plans, and batting strategies.
- Visualize outcomes using predictive analytics.
- Collaborate with others in real time to plan match tactics.

---

## 🌟 Key Features

- 🧠 **AI-Powered Insights:** Suggest optimal tactics based on historical data.
- 🏟️ **Interactive Field Visualization:** Drag and drop fielders, adjust positions, and see expected outcomes.
- 🌀 **Tactical Sandbox:** Try bowling plans, shot selections, batting aggression, and more.
- 🤖 **AI Opponent Mode:** Challenge the AI in simulated matches.
- 🤝 **Collaborative Sessions:** Work with others in real time (SignalR).
- 📚 **Scenario Library:** Save, load, and share your strategies with the community.

---

## ⚙️ Tech Stack

| Layer        | Technologies                                   | Status        |
|--------------|--------------------------------------------------|---------------|
| **Backend**  | ASP.NET Core, C#                                 | Implemented   |
| **Frontend** | React.js, Chart.js, SVG                          | In progress   |
| **AI/ML**    | Azure Cognitive Services, Predictive Analytics   | Planned       |
| **Collab**   | SignalR (WebSockets for real-time updates)       | Planned       |
| **Data**     | CricAPI or historical data; SQL Server (Docker)  | Planned       |

---

## 📁 Project Structure

```
cricksim-pro/
├── backend/        # ASP.NET Core Web API
├── frontend/       # React.js SPA
├── docs/           # Planning, diagrams, references
└── README.md       # You're here!
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js LTS](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio Code](https://code.visualstudio.com/)

### Clone the repo

```bash
git clone https://github.com/NandanDevHub/CrickSim-Pro.git
cd CrickSim-Pro
```

### Run the backend (from /backend)

    cd backend
    dotnet restore
    dotnet build
    dotnet run

### Run the frontend (from /frontend) 
_currently frontend is In Progress_

    cd ../frontend
    npm install
    npm start

Open the URL shown in the terminal to access the React app. Use the form to submit scenarios to the backend and visualize results.

---
## 🔌 API — Latest Request Schema 
_To be used by frontend_

    {
      "GameType": "TEST",
      "PitchType": "Normal",
      "Weather": "Wet",
      "CurrentDay": 1,
      "Overs": 90,
      "BattingFirst": "India",
      "BattingSecond": "Pakistan",
      "TeamAPlayers": [
        { "Name": "Rohit Sharma", "BattingType": "Aggressive", "BowlingType": "None" },
        { "Name": "Yashasvi Jaiswal", "BattingType": "Aggressive", "BowlingType": "None" },
        { "Name": "Virat Kohli", "BattingType": "Anchor",     "BowlingType": "None" },
        { "Name": "Shreyas Iyer", "BattingType": "Finisher",   "BowlingType": "None" },
        { "Name": "Rishabh Pant", "BattingType": "Finisher",   "BowlingType": "None" },
        { "Name": "Hardik Pandya", "BattingType": "AllRounder", "BowlingType": "Fast" },
        { "Name": "Ravindra Jadeja", "BattingType": "AllRounder", "BowlingType": "Spin" },
        { "Name": "Jasprit Bumrah", "BattingType": "Tailender",  "BowlingType": "Fast" },
        { "Name": "Mohammed Shami", "BattingType": "Tailender",  "BowlingType": "Fast" },
        { "Name": "Kuldeep Yadav", "BattingType": "Tailender",  "BowlingType": "Spin" },
        { "Name": "Mohammed Siraj", "BattingType": "Tailender",  "BowlingType": "Fast" }
      ],
      "TeamBPlayers": [
        { "Name": "Babar Azam", "BattingType": "Anchor", "BowlingType": "None" },
        { "Name": "Mohammad Rizwan", "BattingType": "Finisher",   "BowlingType": "None" },
        { "Name": "Fakhar Zaman", "BattingType": "Aggressive", "BowlingType": "None" },
        { "Name": "Abdullah Shafique", "BattingType": "Anchor",     "BowlingType": "None" },
        { "Name": "Saud Shakeel", "BattingType": "Aggressive", "BowlingType": "None" },
        { "Name": "Shadab Khan", "BattingType": "AllRounder", "BowlingType": "Spin" },
        { "Name": "Mohammad Nawaz", "BattingType": "AllRounder", "BowlingType": "Spin" },
        { "Name": "Shaheen Afridi", "BattingType": "Tailender",  "BowlingType": "Fast" },
        { "Name": "Haris Rauf", "BattingType": "Tailender",  "BowlingType": "Fast" },
        { "Name": "Naseem Shah", "BattingType": "Tailender",  "BowlingType": "Fast" },
        { "Name": "Abrar Ahmed", "BattingType": "Tailender",  "BowlingType": "Spin" }
      ],
      "BattingAggression": 50,
      "BowlingAggression": 50
    }

---

## 🛠️ Work in Progress

This is a long-term portfolio project being built from scratch as a complete learning journey—from backend to AI and frontend to collaboration.

---

## 📌 Roadmap

- [x] Define scope and architecture
- [x] Backend: ASP.NET Core API Setup
- [ ] Frontend: React App Setup
- [ ] Cricket simulation engine
- [ ] AI-powered prediction engine
- [ ] Interactive SVG fielding UI
- [ ] Real-time collaboration with SignalR
- [ ] Community strategy sharing module
- [ ] Database persistence (SQL Server / Docker)
- [ ] Final deployment (Azure / Vercel)

---

## 🙌 Contributors

- **Nandan Parmar** — Developer & Architect *(learning by building)*
- **Shivam Singh** — Reviewer

---

## 📄 License

MIT License

---

> 🧠 *CrickSim Pro is not just a project. It's a bold attempt to bring tactical intelligence to cricket fans and professionals alike.*
