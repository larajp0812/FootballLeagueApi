# Football League Frontend (React + Vite)

Single-page React frontend for the Football League API coursework.

## Features

- JWT-based authentication (register/login)
- Protected routes using React Router
- Responsive UI with Bootstrap + React-Bootstrap
- CRUD modules:
  - Teams
  - Players
  - Seasons
  - Venues
  - Matches
  - Match Events
  - Roles (Admin)

## Prerequisites

- Node.js 20+
- .NET backend running locally

## Run Locally

1. Start backend API from project root:

```bash
dotnet run --launch-profile https
```

2. In this frontend folder install dependencies:

```bash
npm install
```

3. Start frontend development server:

```bash
npm run dev
```

4. Open:

- Frontend: `http://localhost:5173`
- Backend Swagger: `https://localhost:7195/swagger`

## API Base URL

The frontend reads `VITE_API_BASE_URL` (fallback: `https://localhost:7195`).

Create `.env` if needed:

```env
VITE_API_BASE_URL=https://localhost:7195
```

## Scripts

- `npm run dev` – start dev server
- `npm run build` – production build
- `npm run preview` – preview production build
- `npm run lint` – run ESLint
- `npm run test` – run unit tests once (Vitest)
- `npm run test:watch` – run tests in watch mode

## Testing

Unit testing uses Vitest + React Testing Library.

Current baseline test:

- `src/components/LoadingState.test.jsx`
