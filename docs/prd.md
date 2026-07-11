# Product Requirements Document (PRD)
## College Control System — Student Information System (SIS)
### Faculty of Engineering, Kafr El-Sheikh University

---

## 1. Overview

**Product Name:** College Control System  
**Type:** Web Application  
**Version:** 1.0  
**Based on:** SRS Document + Credit Hours Regulation 2022 + Student Guide 2025–2026

### 1.1 Purpose

Build a centralized Student Information System (SIS) that automates all academic operations for engineering students on the credit-hours system — from course registration through graduation — strictly enforcing the Faculty's Credit Hours Regulation 2022.

### 1.2 Target Users

| Role | Primary Goal |
|------|-------------|
| **Student** | Manage academic profile, register courses, track graduation progress |
| **Instructor** | Submit grades for assigned course offerings |
| **Advisor** | Review and approve student registration plans |
| **Admin (Control)** | Manage all system data, run end-of-semester engine, generate official reports |

---

## 2. Scope

### In Scope
- User management with Role-Based Access Control (RBAC)
- Academic data management (departments, programs, courses, prerequisites)
- Student course registration (add/drop) with rule enforcement
- Grade entry and calculation
- Automated GPA engine (SGPA, CGPA, academic warnings, dismissal)
- Transcript and report generation
- *(Optional)* AI Chatbot for regulation Q&A

### Out of Scope
- Financial affairs (tuition, fees)
- HR / faculty staff management
- E-Exam systems
- Timetable / schedule management

---

## 3. User Roles & Permissions

### 3.1 Admin
- Create, read, update, and deactivate user accounts; assign roles
- Manage departments, programs, and course catalog
- Define semesters and create course offerings
- Open/close the registration period
- Trigger end-of-semester batch GPA engine
- Run graduation eligibility audits
- Generate official transcripts, graduation certificates, warning lists

### 3.2 Advisor
- View list of assigned student advisees
- Access full academic transcript and status of each advisee
- Approve or reject student registration requests (with reason)
- Add academic notes or warnings to a student's profile

### 3.3 Student
- View and edit personal profile
- Browse course catalog
- Register for / drop courses during the open registration window
- View class schedule, grades, SGPA, CGPA
- Generate and download an unofficial transcript
- Track progress toward graduation requirements

### 3.4 Instructor
- View assigned course offerings
- Access official student roster per offering
- Submit semester work and final exam grades
- Export course rosters and grade sheets

---

## 4. Academic Structure

### 4.1 Programs
Each program (e.g., Intelligent Systems Engineering, Computer Engineering, Civil Engineering) defines:
- Required total credit hours (e.g., 172 for Intelligent Systems Engineering)
- Core and elective course lists

### 4.2 Course Code Structure
All courses follow the format `ABC N1N2N3`:
- **ABC** — 3-letter department code
- **N1** — Academic level (1 = Freshman … 4 = Senior)
- **N2** — Specialization track digit
- **N3** — Sequence number within track

### 4.3 Student Academic Level

| Level | Name | Credit Hours Completed |
|-------|------|----------------------|
| 0 | Freshman | 0% – <20% |
| 1 | Sophomore | 20% – <40% |
| 2 | Junior | 40% – <60% |
| 3 | Senior 1 | 60% – <80% |
| 4 | Senior 2 | 80% – 100% |

---

## 5. Business Rules

### 5.1 Grading Scale (Article 27)

| Score Range | Letter Grade | Grade Points |
|-------------|-------------|-------------|
| ≥ 97% | A+ | 4.0 |
| 93% – <97% | A | 3.7 |
| 89% – <93% | A- | 3.3 |
| 84% – <89% | B+ | 3.0 |
| 80% – <84% | B | 2.7 |
| 76% – <80% | B- | 2.3 |
| 73% – <76% | C+ | 2.0 |
| 70% – <73% | C | 1.7 |
| 67% – <70% | C- | 1.3 |
| 64% – <67% | D+ | 1.0 |
| 60% – <64% | D | 0.7 |
| < 60% | F | 0.0 |

> **Note:** Architect decision — the B+ cap is set at 3.3 points (consistent with Article 28 retake policy) pending final confirmation by the academic committee.

### 5.2 GPA Formulas (Article 30)

```
SGPA = Σ(Grade Points × Credit Hours) for semester courses
       ÷ Total credit hours registered that semester

CGPA = Σ(Grade Points × Credit Hours) for ALL courses since enrollment
       ÷ Total credit hours registered since enrollment
```

**Excluded from GPA calculations:** W (Withdrawn), I (Incomplete), P (Pass)

### 5.3 Course Retake Policy (Article 28)
If a student retakes a **compulsory** course previously failed (F), the maximum grade counted toward GPA is capped at **B+ (3.3)**. The new grade replaces the F in CGPA; all attempts remain visible on the transcript.

### 5.4 Academic Load (Article 12)

| Student CGPA | Max Credit Hours/Semester |
|-------------|--------------------------|
| CGPA ≥ 3.00 | 21 |
| 2.00 ≤ CGPA < 3.00 | 18 |
| CGPA < 2.00 (Warning) | 14 |

> **Exception:** A graduating student may register for 1 additional course beyond their limit.

### 5.5 Academic Warning & Dismissal (Article 33)
- **Warning:** Issued when SGPA < 2.00 in any primary semester.
- **Dismissal:** Issued after **4 consecutive** academic warnings. The system must track the consecutive warning count explicitly.

### 5.6 Prerequisite Rule (Article 11)
A student cannot register for a course unless all prerequisite courses have been passed with a satisfactory grade. The system must block registration and return a clear error message.

### 5.7 Graduation Requirements (Articles 8 & 22)
1. Complete the minimum required credit hours for their program (e.g., 172 for Intelligent Systems Engineering)
2. Achieve CGPA ≥ 2.00
3. Pass Graduation Project 1 and Graduation Project 2 with a grade of C (2.0 points) or higher
4. Complete all required Summer Training with a Pass (P) grade

### 5.8 Attendance / Deprivation (Article 19)
If absence exceeds 25%, the student is barred from the final exam and automatically assigned grade F. The instructor must be able to mark a student as "Deprived."

---

## 6. Functional Requirements

### FR-AUTH — Authentication & Authorization
| ID | Requirement |
|----|-------------|
| AUTH-1 | Secure login screen (username/email + password) |
| AUTH-2 | System identifies user role on login and routes to the correct dashboard |
| AUTH-3 | RBAC enforcement — users cannot access functions outside their role |

### FR-DATA — Core Academic Data (Admin)
| ID | Requirement |
|----|-------------|
| DATA-1 | Admin can manage departments (add/edit/deactivate) |
| DATA-2 | Admin can manage programs and specializations per department |
| DATA-3 | Admin can manage student records (add, edit, link to program) |

### FR-COURSE — Course Management (Admin)
| ID | Requirement |
|----|-------------|
| COURSE-1 | Admin can add/edit courses (code, name, credits, type: compulsory/elective) |
| COURSE-2 | Admin can set prerequisites per course |
| COURSE-3 | Admin can create course offerings (instructor, schedule, max capacity) per semester |

### FR-REG — Registration (Student & Advisor)
| ID | Requirement |
|----|-------------|
| REG-1 | Student sees available course offerings for the current semester |
| REG-2 | Student can add/drop courses only during the open registration period |
| REG-3 | System automatically validates: prerequisite met, credit load not exceeded, no schedule conflict |
| REG-4 | Clear error messages for any rule violation (e.g., "Cannot add — prerequisite CCE211 not met") |
| REG-5 | Registration request is submitted to the assigned academic advisor for approval |
| REG-6 | Advisor can approve or reject (with reason) any pending registration request |

### FR-GRADE — Grade Entry (Instructor)
| ID | Requirement |
|----|-------------|
| GRADE-1 | Instructor can enter semester work and final exam grades for their own offerings only |
| GRADE-2 | System auto-calculates total grade, letter grade, and grade points per regulation |

### FR-ENGINE — Control Engine (Admin)
| ID | Requirement |
|----|-------------|
| ENGINE-1 | Admin can trigger "Run Control Engine" for the current semester after grading is complete |
| ENGINE-2 | Engine calculates SGPA for every student |
| ENGINE-3 | Engine recalculates CGPA for every student (applying retake policy) |
| ENGINE-4 | Engine updates academic status (Good Standing / Warning / Dismissed) and consecutive warning count |
| ENGINE-5 | Engine identifies students who meet graduation requirements |

### FR-REPORT — Reporting
| ID | Requirement |
|----|-------------|
| REPORT-1 | Student can generate and download an **unofficial** transcript (all courses, grades, SGPA, CGPA) |
| REPORT-2 | Admin can generate an **official** transcript |
| REPORT-3 | Admin can generate graduation certificates (program, graduation GPA, honor) |

### FR-AI — AI Chatbot (Optional)
| ID | Requirement |
|----|-------------|
| AI-1 | Chatbot interface available to students |
| AI-2 | Trained exclusively on the Regulation and Student Guide documents |
| AI-3 | Example: "How many hours can I register with a 2.5 GPA?" → answers citing the relevant article |

---

## 7. Non-Functional Requirements

| Category | ID | Requirement |
|----------|----|-------------|
| Security | SEC-1 | All browser–server communication must use HTTPS |
| Security | SEC-2 | Passwords stored as hashed values (never plain text) |
| Performance | PERF-1 | Registration screen must handle 1,000 concurrent users during registration week |
| Performance | PERF-2 | Control Engine must process all students in under 5 minutes |
| Usability | USE-1 | Primary interface language: Arabic; clear and intuitive UI |
| Usability | USE-2 | Compatible with major browsers: Chrome, Firefox, Safari |
| Usability | USE-3 | Responsive design — works efficiently on mobile screens |
| Maintainability | MAINT-1 | Backend code must be well-commented for future team maintainability |
| Reliability | RELIAB-1 | Daily automated database backups |

---

## 8. Data Model (Key Entities)

| Table | Key Fields |
|-------|-----------|
| **Users** | UserID (PK), FirstName, LastName, Email, PasswordHash, Phone |
| **Roles** | RoleID (PK), Name (Admin / Student / Advisor / Instructor) |
| **UserRole** | UserID (FK), RoleID (FK) — many-to-many bridge |
| **Departments** | DepartmentID (PK), DepartmentName, Description |
| **Programs** | ProgramID (PK), ProgramName, DepartmentID (FK), RequiredCredits |
| **Faculty** | FacultyID (PK), UserID (FK), DepartmentID (FK), AcademicRank |
| **Students** | StudentID (PK), UserID (FK), AcademicNumber, ProgramID (FK), AdvisorID (FK), CGPA, AcademicStatus |
| **Courses** | CourseID (PK), Code, Title, DepartmentID (FK), Level, Credits, LectureHours, LabHours |
| **CoursePrerequisites** | CourseID (FK), PrerequisiteCourseID (FK) |
| **CourseOfferings** | OfferingID (PK), CourseID (FK), InstructorID (FK), Semester, Year, MaxCapacity, CurrentEnrolled, Schedule |
| **Registrations** | RegistrationID (PK), StudentID (FK), OfferingID (FK), Status (Pending/Approved/Dropped/Completed), SemesterWorkGrade, FinalExamGrade, TotalGrade, LetterGrade, GradePoints |

### Delete / Deactivate Policy
- **Deactivate only:** Department, Program, Course, Semester, Faculty, AppUser (disable/lock)
- **Status-based:** Student (Active / Graduated / Dismissed)
- **Historical records (no delete, no deactivate):** Registration, Grade, Transcript/GPA
- **Deletable:** Role, Permissions, dummy/test data

---

## 9. Key Workflows

### 9.1 Student Course Registration
1. Validate JWT + Student role
2. Validate CourseOfferingId exists in current open registration period
3. Check all prerequisites passed *(fail → 400 "Prerequisite XYZ not met")*
4. Check credit load not exceeded for current CGPA tier *(fail → 400)*
5. Check offering has available capacity *(fail → 400)*
6. Create `Registration` record with status `Pending` (pending advisor approval)
7. Return `201 Created`

### 9.2 End-of-Semester Control Engine
1. Verify Admin role
2. Fetch all active students with finalized grades not yet processed
3. For each student (inside a DB transaction):
   - Calculate SGPA (exclude W, I, P grades)
   - Recalculate CGPA (apply retake cap policy for failed compulsory courses)
   - Update student CGPA record
4. Apply academic status rules:
   - SGPA < 2.00 → issue Warning, increment consecutive count
   - SGPA ≥ 2.00 → reset consecutive count
   - 4 consecutive warnings → Dismissed
5. Flag students meeting graduation requirements
6. Log results; return `200 OK` with summary report

---

## 10. Architecture Overview

The system follows **Clean Architecture (DDD)**:

```
src/CollegeControl.Domain/          # Aggregates, Entities, Value Objects, Domain Events
src/CollegeControl.Application/     # Commands, Queries, Handlers (CQRS), Validators
src/CollegeControl.Infrastructure/  # EF Core, Repositories, Migrations
src/CollegeControl.Api/             # Controllers, JWT, Swagger, Middleware
SharedKernel/                       # Result<T>, DomainException, ValueObjects
```

**Tech Stack (recommended):**
- Backend: ASP.NET Core Web API, EF Core + PostgreSQL
- Auth: JWT (Identity + custom Users table, TPT inheritance)
- Logging: Serilog
- Docs: Swagger UI
- Containerization: Docker Compose (API + Postgres + pgAdmin)

---

## 11. 12-Week Delivery Roadmap

| Week | Focus | Key Deliverables |
|------|-------|-----------------|
| 1 | Solution Setup + Architecture Skeleton | Clean Architecture folders, Docker Compose, Swagger, CI pipeline |
| 2 | Domain Layer (DDD Core) | All aggregates, value objects, domain events, invariant rules |
| 3 | Infrastructure + EF Core Modeling | TPT mapping, all repositories, first migration, seed data |
| 4 | Authentication & Authorization | JWT, RBAC policies, Login/Refresh/Register endpoints |
| 5 | Admin Module | CRUD for Departments, Programs, Courses, Prerequisites |
| 6 | Course Offering Module | Create/update offerings, capacity management, schedule conflict detection |
| 7 | Student Registration Module | Full registration flow with all business rule validations |
| 8 | Advisor Workflow | View advisees, approve/reject registrations |
| 9 | Instructor Grading Module | Grade submission, letter grade auto-assignment, deprivation flag |
| 10 | GPA Engine (Control Engine) | SGPA/CGPA calculation, retake policy, academic status updates |
| 11 | Graduation + Reporting | Graduation eligibility audit, transcript endpoint, certificates |
| 12 | Polish, Security & Deployment | Performance tuning, rate limiting, CI/CD, load testing, final docs |

---

## 12. Open Issues / Decisions Pending

| # | Issue | Status |
|---|-------|--------|
| 1 | B+ GPA cap (3.3 vs other interpretation in source documents) | Pending academic committee confirmation — defaulting to 3.3 |
| 2 | Exact score breakpoints for grading scale | Confirmed from Article 27 |
| 3 | Optional AI Chatbot scope and LLM choice | Deferred — subject to budget/time |

---

*Document generated from SRS: College Control System v1.0*  
*Last updated: April 2026*