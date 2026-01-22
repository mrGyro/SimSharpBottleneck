# 🏭 SimSharp Bottleneck Detection Demo

> Discrete-Event Simulation of a production line demonstrating the local optimization paradox and Theory of Constraints.

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![SimSharp](https://img.shields.io/badge/SimSharp-3.4.2-green)](https://github.com/abeham/SimSharp)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## 📋 About

This project demonstrates how **improving a single process doesn't always improve the entire system**. 

Using discrete-event simulation (SimSharp), we model a production line and show:
- 🎯 How to identify bottlenecks
- ❌ Why optimizing non-bottleneck processes is futile
- ✅ How proper improvement yields +18% throughput

**Applications:** manufacturing, DevOps, business processes, any sequential systems.

---

## 🚀 Quick Start

### Requirements
- .NET 8.0 SDK or higher
- Visual Studio Code or Visual Studio

### Installation and Run

```bash
git clone <your-repo>
cd SimSharp/SimSharpExample
dotnet restore
dotnet run
```

Results will be saved in the `Results/` folder (CSV files).

---

## 🏭 System Model

**Production Line:**

```
[Items] → [Cutting] → [Assembly] → [Testing] → [Packaging] → [Complete]
           8 min       12 min        10 min        6 min
                         ↑
                    BOTTLENECK!
```

**Simulation Parameters:**
- 4 sequential stations
- 1 machine per station
- 20 items
- Arrival interval: 5 minutes

---

## 📊 Results

### Scenario Comparison

| Scenario | Description | Throughput | Change | Lead Time |
|----------|----------|------------|-----------|-----------|
| **Baseline** | Standard configuration | 4.55 items/h | - | 102 min |
| **ImprovedCutting** | Cutting +20% faster | 4.57 items/h | +0.6% ❌ | 101 min |
| **BottleneckFixed** | Assembly +20% faster | 5.37 items/h | **+18%** ✅ | 81 min |
| **Stochastic** | Realistic variability | 4.53 items/h | -0.4% | 116 min |

### Utilization Visualization

```
Assembly  ████████████████████████████████████████████  90.9% ← Bottleneck!
Testing   ████████████████████████████████████          75.8%
Cutting   ███████████████████████████                   60.6%
Packaging ████████████████████                          45.5%
```

---

## 💡 Key Findings

### ❌ Paradox: Improving non-bottleneck

Improved **Cutting** by 20%:
- Throughput: +0.6% (virtually zero)
- Lead Time: barely changed
- **Conclusion:** Useless!

### ✅ Correct Approach: Improving bottleneck

Improved **Assembly** by 20%:
- Throughput: **+18.1%** 🚀
- Lead Time: **-20.9%** 🚀
- **Conclusion:** Significant system-wide effect!

### 🎯 Theory of Constraints (TOC)

1. Identify bottleneck (Assembly: 90.9% utilization)
2. Exploit bottleneck (prevent idle time)
3. Subordinate everything to bottleneck
4. **Improve only the bottleneck**
5. Repeat the process

---

## 📁 Project Structure

```
SimSharpExample/
├── Program.cs              # Entry point
├── Simulation/
│   ├── Item.cs            # Item model
│   ├── Station.cs         # Station model
│   └── ProductionLine.cs  # Production line
├── Metrics/
│   ├── MetricsCollector.cs      # Metrics collection
│   ├── CsvExporter.cs           # CSV export
│   └── ComparisonAnalyzer.cs    # Comparative analysis
├── Scenarios/
│   ├── BaselineScenario.cs       # Base configuration
│   ├── ImprovedStationScenario.cs # Non-bottleneck improvement
│   ├── BottleneckFixedScenario.cs # Bottleneck improvement
│   └── StochasticScenario.cs      # Stochastic model
└── Utils/
    └── RandomDistributions.cs    # Random distributions
```

---

## 📈 Metrics

The project collects the following metrics:

- **Utilization** - percentage of station busy time
- **Throughput** - items per hour
- **Lead Time** - time item spends in system
- **Queue Length** - queue size
- **WIP** - Work In Progress

All metrics are exported to CSV for further analysis.

---

## 🎓 Concepts

### Discrete-Event Simulation (DES)
- Events occur at specific points in time
- Time model: simulation time (not real time)
- Processes: coroutines with `yield return`

### Theory of Constraints (TOC)
- Any system is limited by a bottleneck
- Improving non-bottleneck doesn't improve system
- Focus on system performance, not local

### Key Distributions
- **Exponential** - for random arrivals
- **Normal** - for processing time with variability

---

## 🔧 Technologies

- **SimSharp 3.4.2** - discrete-event simulation library
- **C# 12** - programming language
- **.NET 8.0** - platform
- **CSV Export** - for analysis in Excel/Python

---

## 📊 Using Results

CSV files can be imported into:
- **Excel** - for charts
- **Python (pandas, matplotlib)** - for detailed analysis
- **Power BI / Tableau** - for interactive dashboards

---

## 🎯 Real-World Applications

This model applies to:

- ✅ Manufacturing lines
- ✅ CI/CD pipelines
- ✅ Support ticket processing
- ✅ Sequential business processes
- ✅ Any systems with bottlenecks

**Conclusion:** Don't optimize locally. Find the bottleneck and improve it!

---

## 📝 License

This project is created for educational purposes.

---

## 🤝 Contact

Created to demonstrate SimSharp and discrete-event simulation concepts.

**Suitable for:**
- LinkedIn portfolio
- Technical interviews
- DES learning
- Demonstrating C# and simulation skills

---

**⭐ If this project was helpful, give it a star!**
