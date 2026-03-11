# 📋 Unity Programmer Technical Task | NG+ Assessment

![Unity](https://img.shields.io/badge/Unity-6000.2.15f1-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-7.3+-purple?logo=csharp)
![Status](https://img.shields.io/badge/Status-Completed-success)
![Deadline](https://img.shields.io/badge/Timeframe-48_Hours-red)

This repository contains my solution for the **Unity Programmer Task** assigned by **NG+**. The project was developed within a 48-hour window, focusing on technical proficiency, code organization, and high-quality game feel.

## 🎯 Task Objectives & Requirements

According to the brief, the evaluation focused on four key areas where I implemented the following:

1. **Technical Excellence:** Developed in **Unity 6 (6000.2.15f1)**, ensuring full compatibility with the latest engine features.
2. **Code Organization:** Applied a strict file structure and assembly definitions to ensure project scalability.
3. **Open-ended Problem Solving:** Interpreted general requirements to build a cohesive and polished prototype.
4. **Aesthetics & Design:** Beyond pure programming, I integrated VFX, lighting, and UI to deliver a "comprehensive user experience" as requested.

## 🏗️ Architectural Implementation

To demonstrate senior-level engineering, I implemented a **Service-Oriented Architecture**:

* **Modular Bootstrapper:** A centralized entry point that handles scene initialization and dependency injection, avoiding the common pitfalls of Unity's execution order.
* **Character Controller:** A custom-built controller focused on "snappy" and responsive movement.
* **State Management:** Used a Finite State Machine (FSM) to manage game flow (Menu, Gameplay, Results).
* **Observer Pattern:** Implemented for the UI and Game Events to ensure that the core logic remains independent of the visual representation.

## 🛠️ Extra Features (Above & Beyond)

The brief encouraged "showcasing creativity." I included several **Suggested Additional Features** to elevate the prototype:

* **World Building:** Crafted a curated level design with balanced lighting and environmental storytelling.
* **Advanced UI Toolkit:** Developed a reactive HUD that provides real-time feedback on player performance.

## 🔧 Technical Deep Dive

* **Clean Code & SOLID:** Every class has a single responsibility. Methods are documented, and naming conventions follow professional C# standards.
* **Asset Management:** Optimized use of prefabs and ScriptableObjects for data configuration, making it easy to tweak game values without changing code.
* **Performance:** Frustum culling and occlusion culling were implemented to try to ensure a smooth experience.

## 📂 Project Structure

* **/Core**: Architecture, Bootstrappers, and Global Services.
* **/Gameplay**: Character logic, interactions, and state machines.
* **/Art & VFX**: Shaders, particles, and environment assets.
* **/UI**: UXML/USS files and View Controllers.

## 👨‍💻 Author
<div aling="left">  
  <h4>Jesús Carrero - Unity Gameplay Engineer</h1>
  <a href="https://jesuscarrero.netlify.app/">
    <img src="https://img.shields.io/badge/Portfolio-a83333?style=for-the-badge&logo=netlify&logoColor=white" width="150" />
  </a>
</div>
