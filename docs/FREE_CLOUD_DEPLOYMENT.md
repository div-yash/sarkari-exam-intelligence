# ☁️ 100% Free Production Deployment (Neon.tech + Render.com + Vercel)

Host the entire **Sarkari Exam Intelligence** stack publicly at zero cost:

---

## 1. Database (Neon.tech or Supabase Free PostgreSQL)
1. Register for a free PostgreSQL database at **[Neon.tech](https://neon.tech/)** (or Supabase).
2. Copy your PostgreSQL connection string from the Neon dashboard:
   - You can use **either** the raw Neon URI directly (`postgresql://user:password@ep-xyz.ap-southeast-1.aws.neon.tech/neondb?sslmode=require`)
   - **Or** the key-value format:
     ```text
     Host=ep-xyz.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=YOUR_PASSWORD;Port=5432;SSL Mode=Require;Trust Server Certificate=true;
     ```
   - The backend automatically detects PostgreSQL URIs (`postgres://` / `postgresql://`) or `Host=...` strings and converts URIs cleanly for `Npgsql`.

---

## 2. Backend API (Render.com Free Web Service)
1. Push this repository to **GitHub**.
2. Sign up at **[Render.com](https://render.com/)** → **New + → Web Service** → connect your GitHub repository.
3. Configure service settings:
   - **Runtime:** `Docker` *(Render automatically detects the root [`Dockerfile`](../../Dockerfile))*
   - **Instance Type:** `Free`
   - **Health Check Path:** `/api/health`
4. Add **Environment Variables**:
   - `ConnectionStrings__DefaultConnection` = *(your Neon.tech connection string)*
   - `Jwt__Key` = *(a random 32+ character secret key)*
   - `EnableSwagger` = `true` *(optional — exposes `/swagger` for API testing)*
   - `CORS_ALLOWED_ORIGINS` = `https://your-vercel-app.vercel.app` *(update right after step 3)*
5. Click **Deploy Web Service**. Render hosts your backend at a public URL such as `https://sarkari-exam-api.onrender.com` and automatically initializes all tables + 65 exams + 26 syllabus subjects on first boot.
   - *Note:* Render Free instances spin down after 15 minutes of inactivity; the first cold wake-up request takes ~50 seconds.

---

## 3. Frontend App (Vercel Free Hosting)
1. Open [`frontend/src/environments/environment.prod.ts`](../../frontend/src/environments/environment.prod.ts) and set `apiBaseUrl` to your Render backend URL (no trailing `/api`):
   ```ts
   export const environment = {
     production: true,
     apiBaseUrl: 'https://sarkari-exam-api.onrender.com'
   };
   ```
2. Commit and push your changes to GitHub.
3. Sign up at **[Vercel](https://vercel.com/)** → **Add New → Project** → import your GitHub repository:
   - **Framework Preset:** `Angular`
   - **Root Directory:** `frontend`
   - **Build Command:** `npm run build`
   - **Output Directory:** `dist/frontend/browser`
   *(Client-side Angular routing rewrites are preconfigured in [`frontend/vercel.json`](../../frontend/vercel.json).)*
4. Click **Deploy**.
5. Copy your Vercel public domain (e.g., `https://sarkari-exam-intelligence.vercel.app`), paste it into Render's `CORS_ALLOWED_ORIGINS` environment variable, and trigger a quick Render redeploy/restart.
