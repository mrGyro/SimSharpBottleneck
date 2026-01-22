# 📊 Production Line Simulation Analysis

## 🎯 Project Goal

Demonstrate the **local optimization paradox** in production systems using discrete-event simulation (SimSharp).

---

## 🏭 System Model

**Production line with 4 stations:**
1. **Cutting** - 8 minutes
2. **Assembly** - 12 minutes ← **BOTTLENECK**
3. **Testing** - 10 minutes
4. **Packaging** - 6 minutes

**Parameters:**
- 20 items
- Arrival interval: 5 minutes
- Capacity: 1 machine per station

---

## 📈 Scenario Results

| Scenario | Throughput | Lead Time | Assembly Util | Change |
|----------|------------|-----------|---------------|--------|
| **Baseline** | 4.55 items/h | 102.5 min | 90.9% | - |
| **ImprovedCutting** | 4.57 items/h | 100.9 min | 91.5% | +0.6% 😐 |
| **BottleneckFixed** | 5.37 items/h | 81.1 min | 85.9% | +18.1% 🚀 |
| **Stochastic** | 4.53 items/h | 116.3 min | 90.2% | Realistic variability |

---

## 💡 Key Findings

### ❌ Paradox: Improving non-bottleneck

**Scenario:** Improved Cutting by 20% (8 min → 6.4 min)

**Result:**
- Throughput: +0.6% (virtually zero!)
- Lead Time: -1.6% (minimal improvement)
- Assembly utilization **increased** to 91.5%

**Why doesn't it work?**
- Cutting supplies items faster to Assembly
- Assembly (bottleneck) is still slow
- Queues before Assembly grow
- **System is limited by the slowest link!**

---

### ✅ Correct Approach: Improving bottleneck

**Scenario:** Improved Assembly by 20% (12 min → 9.6 min)

**Result:**
- Throughput: **+18.1%** (significant increase!)
- Lead Time: **-20.9%** (substantial decrease!)
- Assembly utilization decreased to 85.9%

**Why does it work?**
- Assembly is the system's bottleneck
- Improving bottleneck speeds up entire system
- Queues shrink
- **System-wide effect!**

---

### 🎲 Stochastic Model

**Features:**
- Random processing time (Normal distribution, σ=20%)
- Random item arrival (Exponential distribution)
- Lead Time: 116.3 min (higher due to variability)
- Some items wait up to 3+ hours!

**Conclusion:** Real systems always have variability, which increases Lead Time.

---

## 🎓 Theory of Constraints (TOC)

**Core Principles:**

1. **Identify the bottleneck** 
   - In our case: Assembly (90.9% utilization)

2. **Exploit the bottleneck**
   - Ensure it never stays idle

3. **Subordinate everything to the bottleneck**
   - Don't overproduce at other stations

4. **Elevate the bottleneck**
   - Only this will have system-wide effect

5. **Repeat** 
   - Bottleneck may shift!

---

## 📊 Efficiency Metrics

### Utilization (Load)
- **Assembly: 90.9%** ← bottleneck
- Cutting: 60.6%
- Testing: 75.8%
- Packaging: 45.5%

### Throughput (Capacity)
- Baseline: 4.55 items/hour
- After bottleneck improvement: **5.37 items/hour** (+18%)

### Lead Time (Time in system)
- Baseline: 102.5 minutes
- After bottleneck improvement: **81.1 minutes** (-21%)

---

## 🚀 Practical Recommendations

### ✅ DO:
1. Measure utilization of all resources
2. Identify bottlenecks (>85% utilization)
3. Invest in bottleneck improvements
4. Monitor Lead Time and WIP
5. Use simulation before real changes

### ❌ DON'T:
1. Improve all stations simultaneously
2. Invest in underutilized resources
3. Ignore variability
4. Locally optimize without system view
5. Increase arrival rate without increasing bottleneck capacity

---

## 📁 Result Files

- `Results/Baseline/` - base configuration
- `Results/ImprovedCutting/` - non-bottleneck improvement
- `Results/BottleneckFixed/` - bottleneck improvement
- `Results/Stochastic/` - stochastic model
- `Results/comparison.csv` - comparison table

---

## 🎯 Final Conclusion

**Local Optimization ≠ Global Optimization**

Improving individual processes doesn't guarantee system-wide improvement. 
Invest resources in bottlenecks, not in already fast processes.

**Applicable to:**
- Manufacturing lines
- DevOps pipelines
- Business processes
- Any sequential systems

---

*Created using SimSharp - discrete-event simulation library for C#*
