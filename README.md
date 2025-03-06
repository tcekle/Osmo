# OSMO - Open Source Manufacturing Orchestrator

## Overview

OSMO (Open Source Manufacturing Orchestrator) is a flexible and extensible **Manufacturing Execution System (MES)** designed to manage and optimize manufacturing operations. Built with **.NET 9.0**, it ensures high performance, scalability, and cross-platform compatibility.

## Key Features

- **Open-Source MES Application** – Monitor and manage manufacturing floor operations efficiently.
- **Extensible Plugin Architecture** – Customize and extend functionalities for specific manufacturing needs.
- **Customization & Scalability** – Adapt to business growth and evolving requirements.
- **Community-Driven Development** – Continuous innovation through open-source contributions.
- **Integration Friendly** – Seamlessly connects with manufacturing systems, databases, and IoT devices.
- **Real-Time Operations Management** – Optimize production efficiency with live tracking.
- **PostgreSQL Support** – Uses an open-source database, compatible with TimescaleDB for time-series data.
- **AGPLv3 Licensed** – Ensures modifications remain open-source while allowing for commercial licensing.

## Core Concepts

- **Producer** – A component that generates information (e.g., LumenX, PSV systems, ConneX, Board testing stations).
- **Consumer** – A component that consumes information (e.g., Email alerts, Process validation, Efficiency calculators).
- **Unit** – A trackable manufacturing component (e.g., Programmable devices, Reels of devices).
- **Activity** – Actions within the process flow (e.g., Setup, Maintenance, Soldering).
- **Flow** – Series of activities representing production steps (e.g., Order received → Program parts → Assemble → Ship).

## Getting Started

### Prerequisites
- .NET 9.0
- PostgreSQL
- RabbitMQ

## Contributing
We welcome contributions from the community! Please follow these steps:

1. Fork the repository.
1. Create a new feature branch.
1. Commit your changes.
1. Open a pull request for review.

## Contact & Support
For discussions, support, and contributions, visit the OSMO GitHub issues.
