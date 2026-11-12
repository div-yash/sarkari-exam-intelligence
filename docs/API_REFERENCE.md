# API Reference (Summary)

All endpoints wrap responses in `{ "success": true, "message": "...", "data": ... }` (except `/api/calendar/export.ics` which returns `text/calendar`). Interactive OpenAPI documentation is available at `/swagger`.

| Module | Method & Route | Auth |
| --- | --- | --- |
| Health | `GET /api/health` | Public |
| Auth | `POST /api/auth/register`, `POST /api/auth/login`, `GET /api/auth/me` | Public / JWT |
| Profile | `GET /api/profile`, `PUT /api/profile` | JWT |
| Exams | `GET /api/exams/categories`, `GET /api/exams`, `GET /api/exams/{slug}`, `GET /api/exams/by-id/{id}` | Public (enriched if JWT) |
| Eligibility | `GET /api/eligibility/my`, `GET /api/eligibility/my/{examId}`, `POST /api/eligibility/evaluate` | JWT / Public |
| Dashboard | `GET /api/dashboard/summary` | Public (enriched if JWT) |
| Calendar | `GET /api/calendar/events`, `GET /api/calendar/export.ics` | Public |
| Syllabus | `GET /api/syllabus`, `PUT /api/syllabus/topics/{topicId}/progress` | Public / JWT |
| Preparation | `GET /api/preparation/overview`, `PUT /api/preparation/study-plan`, `POST /api/preparation/mock-tests`, `DELETE /api/preparation/mock-tests/{id}`, `GET /api/preparation/applications`, `PUT /api/preparation/applications` | JWT |
| Watchlist | `GET /api/watchlist`, `POST /api/watchlist`, `DELETE /api/watchlist/{examId}` | JWT |
| Analytics | `GET /api/analytics/vacancies`, `GET /api/analytics/exams/{examId}/history` | Public |
| Notifications | `GET /api/notifications`, `GET /api/notifications/unread-count`, `POST /api/notifications/{id}/read`, `POST /api/notifications/read-all`, `GET /api/notifications/preferences`, `PUT /api/notifications/preferences`, `POST /api/notifications/run-check` | JWT |
| Admin | `GET /api/admin/overview`, `GET /api/admin/lookups`, `POST /api/admin/exams`, `PUT /api/admin/exams/{id}`, `PUT /api/admin/exams/{id}/dates`, `PUT /api/admin/exams/{id}/vacancies`, `POST /api/admin/exams/{id}/history`, `POST /api/admin/exams/{id}/syllabus`, `POST /api/admin/exams/{id}/status`, `POST /api/admin/exams/{id}/approve`, `POST /api/admin/exams/{id}/upload-pdf`, `GET /api/admin/sources`, `POST /api/admin/sources/{id}/verify`, `GET /api/admin/audit-logs`, `GET /api/admin/users`, `POST /api/admin/users/{id}/toggle-active`, `POST /api/admin/notifications/dispatch-check` | Admin Role |
