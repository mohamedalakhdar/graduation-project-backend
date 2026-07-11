# Missing Endpoints

Endpoints needed but not yet implemented, mapped to PRD requirements and priority.

## 3. Student Graduation (Manual) (FR-ENGINE-5 / FR-REPORT-3)

The control engine auto-identifies graduates, but there is no manual "mark as graduated" endpoint or graduation certificate generation.

| #   | Method | Route                                       | Action                          | PRD Ref  | Priority |
| --- | ------ | ------------------------------------------- | ------------------------------- | -------- | -------- |
| 6   | `PUT`  | `/api/students/{id}/graduate`               | Manually graduate a student     | ENGINE-5 | Medium   |
| 7   | `GET`  | `/api/students/{id}/graduation-certificate` | Download graduation certificate | REPORT-3 | Medium   |

> **Why:** Admin may need to override graduation status. Certificate download is listed as a requirement.

---

## 4. Student Current Load (Article 12)

`Student.GetMaxAllowedCreditHours()` already implements the rule, but no endpoint exposes it.

| #   | Method | Route                             | Action                              | PRD Ref | Priority |
| --- | ------ | --------------------------------- | ----------------------------------- | ------- | -------- |
| 8   | `GET`  | `/api/students/{id}/current-load` | Get enrolled vs max allowed credits | Art. 12 | **High** |

> **Why:** Registration UI needs to show the student their remaining capacity. Easy win — logic already exists in the domain.

---

## 5. Attendance & Deprivation (Article 19 / FR-GRADE)

PRD GRADE says the instructor must be able to mark a student as "Deprived." Currently no endpoint exists for this.

| #   | Method | Route                             | Action                                   | PRD Ref         | Priority |
| --- | ------ | --------------------------------- | ---------------------------------------- | --------------- | -------- |
| 9   | `PUT`  | `/api/registrations/{id}/deprive` | Mark student as deprived (absence > 25%) | GRADE / Art. 19 | Medium   |

> **Why:** Article 19 requires this; currently no way to record it.

---

## 6. Department Resource Browsing

No way to get a department's associated courses, faculty, or students in one call.

| #   | Method | Route                            | Action                          | PRD Ref | Priority |
| --- | ------ | -------------------------------- | ------------------------------- | ------- | -------- |
| 10  | `GET`  | `/api/departments/{id}/courses`  | Courses belonging to department | DATA-1  | Medium   |
| 11  | `GET`  | `/api/departments/{id}/faculty`  | Faculty members in department   | DATA-1  | Medium   |
| 12  | `GET`  | `/api/departments/{id}/students` | Students in department programs | DATA-1  | Medium   |

> **Why:** Admin dashboard needs department-level browsing.

---

## 7. Instructor Schedule

Instructor has no way to see their teaching schedule via API.

| #   | Method | Route                        | Action                                        | PRD Ref | Priority |
| --- | ------ | ---------------------------- | --------------------------------------------- | ------- | -------- |
| 13  | `GET`  | `/api/faculty/{id}/schedule` | Get instructor's course offerings by semester | —       | Medium   |

> **Why:** Instructors need to see what/where they teach. The `GET /api/faculty/{id}/courses` endpoint exists but returns course IDs only, not offering details.

---

## 8. Academic Reports (Enhancements)

| #   | Method | Route                             | Action                                                | PRD Ref | Priority |
| --- | ------ | --------------------------------- | ----------------------------------------------------- | ------- | -------- |
| 14  | `GET`  | `/api/reports/grade-distribution` | Grade distribution across all offerings in a semester | REPORT  | Low      |
| 15  | `GET`  | `/api/reports/semester-summary`   | Summary of registrations, pass rates per semester     | REPORT  | Low      |

> **Why:** Useful for admin dashboards but lower priority than core functionality.

---

### 8.1. System Configuration

| #   | Method | Route                       | Action                             | PRD Ref | Priority |
| --- | ------ | --------------------------- | ---------------------------------- | ------- | -------- |
| 16  | `PUT`  | `/api/config/grading-scale` | Override grading scale breakpoints | —       | Low      |
| 17  | `GET`  | `/api/config/grading-scale` | Get current grading scale          | Art. 27 | Low      |

> **Why:** The grading scale is currently hard-coded in `Grade.Create()`. Only needed if flexibility is required.

---

## 10. Full CRUD Gaps — Entity by Entity

---

<!-- ### 10d. Registration — DELETE + List All

Registration has state-machine updates but no `DELETE` and no "list all" endpoint.

| #   | Method   | Route                     | Action                                                                                         | PRD Ref | Priority |
| --- | -------- | ------------------------- | ---------------------------------------------------------------------------------------------- | ------- | -------- |
| 23  | `DELETE` | `/api/registrations/{id}` | Delete > **Why:** Admin needs to remove erroneous registrations. No endpoint exists to browse all registrations globally. -->

---
