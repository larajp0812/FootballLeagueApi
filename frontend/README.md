# Football League Frontend

A modern React single-page application (SPA) that provides a web interface for managing football leagues, teams, players, matches, and seasons. This frontend consumes a .NET 8 backend API and provides responsive design for both desktop and mobile devices.

## Features

- **User Authentication**: JWT-based login and registration with role-based access control
- **Team Management**: Create, read, update, and delete teams and team information
- **Player Management**: Manage player profiles including positions and statistics
- **Match Management**: Schedule and track matches with match events
- **Season Management**: Organize teams and matches by seasons
- **League Table**: View standings calculated by the backend API
- **Admin Dashboard**: Role-based admin interface for managing users and roles
- **Responsive Design**: Fully responsive UI optimized for mobile, tablet, and desktop
- **Real-time Loading States**: Loading indicators and error handling throughout the app

## Tech Stack

- **React 19** - UI library
- **React Router 7** - Client-side routing for SPA navigation
- **React Bootstrap 2** - UI component library for responsive design
- **Vite 7** - Fast build tool and development server
- **Vitest 4** - Unit testing framework
- **ESLint 9** - Code quality and style enforcement

## Prerequisites

- Node.js >= 18.x
- npm >= 9.x
- The backend API running on `https://localhost:5240` (see backend README for setup)

## Installation

1. Navigate to the frontend directory:

```bash
cd frontend
```

2. Install dependencies:

```bash
npm install
```

## Configuration

### Environment Variables

Create a `.env.local` file in the frontend directory (optional - defaults are provided):

```env
VITE_API_BASE_URL=https://localhost:5240
```

The frontend will default to `https://localhost:5240` if no environment variable is set.

For production builds, set `VITE_API_BASE_URL` to your deployed API host (no `/swagger` path), for example:

```env
VITE_API_BASE_URL=https://ljp-football-league-avh9c5h2gcawctcv.canadacentral-01.azurewebsites.net
```

## Development

Start the development server:

```bash
npm run dev
```

The app will be available at `http://localhost:5173/` and will automatically reload when you make changes.

## Building

Create a production build:

```bash
npm run build
```

The optimized build will be output to the `dist/` directory.

## Previewing Production Build

Preview the production build locally:

```bash
npm run preview
```

## Testing

### Run Tests

Execute all tests once:

```bash
npm test
```

### Watch Mode

Run tests in watch mode (re-runs tests when files change):

```bash
npm test:watch
```

## Linting

Check code quality and style compliance:

```bash
npm run lint
```

Fix auto-fixable linting issues:

```bash
npm run lint -- --fix
```

## Project Structure

```
frontend/
├── src/
│   ├── components/          # Reusable UI components
│   ├── pages/              # Page components for routes
│   ├── services/           # API client and service functions
│   ├── contexts/           # React Context for global state (Authentication)
│   ├── hooks/              # Custom React hooks
│   ├── App.jsx             # Main app component with routing
│   └── main.jsx            # Application entry point
├── public/                 # Static assets
├── index.html              # HTML template
├── vite.config.js          # Vite configuration
├── eslint.config.js        # ESLint configuration
├── package.json            # Dependencies and scripts
└── README.md               # This file
```

## Key Pages

- **Login** (`/login`) - User authentication
- **Register** (`/register`) - Account registration
- **Dashboard** (`/`) - Main dashboard with overview
- **Teams** (`/teams`) - Team management (CRUD operations)
- **Players** (`/players`) - Player management (CRUD operations)
- **Seasons** (`/seasons`) - Season management
- **Matches** (`/matches`) - Match scheduling and management
- **Match Events** (`/matchevents`) - Track match events and scores
- **League Table** (`/table`) - View current standings
- **Roles** (`/roles`) - Admin role management (Admin only)

## API Endpoints Used

The frontend consumes the following backend API endpoints:

- **Authentication**: `/api/auth/login`, `/api/auth/register`, `/api/auth/refresh`, `/api/auth/forgot-password`, `/api/auth/reset-password`, `/api/auth/confirm-email`
- **Teams**: `GET/POST/PUT/DELETE /api/teams`
- **Players**: `GET/POST/PUT/DELETE /api/players`
- **Seasons**: `GET/POST/PUT/DELETE /api/seasons`
- **Matches**: `GET/POST/PUT/DELETE /api/matches`
- **Match Events**: `GET/POST/PUT/DELETE /api/matchevents`
- **Standings**: `GET /api/standings`
- **Roles**: `GET/POST/PUT/DELETE /api/roles` (Admin)
- **Users**: `GET /api/users` (Admin)
- **Health**: `GET /health` - API health check

## Authentication

The app uses JWT (JSON Web Token) authentication. After login, the token is stored in local storage and included in all API requests. The frontend automatically handles:

- Token expiration and session timeout
- Role-based access control (User/Admin)
- Protected routes that require authentication

## State Management

Global state is managed using React Context API:

- **AuthContext**: Manages user authentication state, token, role, and auth methods

Component-level state is managed with React's `useState` hook.

## Error Handling

The frontend includes comprehensive error handling:

- API request errors are caught and displayed to users
- Loading states prevent multiple simultaneous requests
- Form validation provides immediate feedback
- Error alerts allow users to dismiss and retry operations

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Performance Optimizations

- Code splitting via Vite's dynamic imports
- Tree-shaking of unused code
- CSS minification and optimization
- Image optimization recommendations
- Lazy loading of routes

## Contributing

When contributing to this project:

1. Follow the existing code style (enforced by ESLint)
2. Write tests for new components and services
3. Ensure all tests pass before committing
4. Use meaningful commit messages
5. Keep components focused and reusable

## Related Documentation

- [Backend API README](../README.md)
- [React Documentation](https://react.dev)
- [React Router Documentation](https://reactrouter.com)
- [Bootstrap Documentation](https://react-bootstrap.github.io)
- [Vite Documentation](https://vitejs.dev)

## Azure Static Web Apps Notes

- If you use a strict Content Security Policy, ensure `connect-src` allows your API host.
- In this project, CSP is configured in `staticwebapp.config.json`.
- After configuration changes, redeploy and hard-refresh the browser to pick up updated headers.

## Submission Evidence Checklist

Use this checklist before submission and viva:

- Build passes: `npm run build`
- Tests pass: `npm test`
- Lint passes: `npm run lint`
- API base URL configured for production (`VITE_API_BASE_URL`)
- Mobile and laptop screenshots captured for key pages (Dashboard, Teams, Players, Matches, Login)
- Authentication flow demonstrated (register -> confirm email -> login)
- Session restore and token refresh demonstrated
- Error/loading states demonstrated on at least one CRUD page
- GitHub repository contains clean README and setup steps

## License

This project is provided for educational and portfolio purposes.
