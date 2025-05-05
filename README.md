# Clinic Management API

A .NET Core Web API for managing patients in a clinic.

## Features

- Complete CRUD operations for patients (Create, Read, Update, Delete)
- Patient gift collection from external API
- File-based patient storage
- Swagger documentation
- Error handling and logging

## Project Structure

- **ClinicManagement.API**: Main Web API project
- **ClinicManagement.PatientManager**: Class library for patient management

## Setup Instructions

1. Clone the repository
2. Open the solution in Visual Studio or your preferred IDE
3. Build the solution
4. Run the API project

## API Endpoints

### Patients

- `POST /api/patients` - Create a new patient
- `GET /api/patients` - Get all patients
- `GET /api/patients/{ci}` - Get a patient by CI
- `PUT /api/patients/{ci}` - Update a patient
- `DELETE /api/patients/{ci}` - Delete a patient

### Gifts

- `GET /api/gifts` - Get gifts for patients

## Development

This project follows the 12-Factor App methodology:

- **Codebase**: Single codebase tracked in Git
- **Dependencies**: Explicitly declared and isolated
- **Config**: Configuration stored in environment variables
- **Logs**: Logs treated as event streams with proper error handling