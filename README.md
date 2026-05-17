# Customer Order Support Chatbot Using Tool Calling

## Overview
This project is an AI-inspired customer support chatbot built using ASP.NET Core Web API, repository pattern, SQL stored procedures, and tool-calling architecture.

The chatbot identifies user intent, selects the correct backend tool/API, retrieves customer data, and returns a natural response.

---

## Features

- Order Summary
- Payment Status
- Shipment Tracking
- Refund / Return Status
- Customer Overview
- Customer Order History
- Pending Refund Reports

---

## Architecture

User Query
↓
Intent Detection
↓
Tool Selection
↓
Repository Layer
↓
Stored Procedure / Database
↓
Natural Language Response

---

## Tech Stack

- ASP.NET Core Web API
- C#
- Entity Framework Core
- MySQL
- Stored Procedures
- Swagger
- Tool Calling Architecture

---

## Future Enhancements

- OpenAI Tool Calling
- AWS Bedrock Integration
- Semantic Search
- RAG Architecture
- React Frontend
- Authentication & Authorization
- Docker Deployment

---

## Example Queries

- Where is order 102?
- Did customer 5 pay?
- Show shipment details for order 88
- What did customer 7 purchase?
- Show pending refunds
