import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import StudentsPage from './pages/StudentsPage';
import StudentEventsPage from './pages/StudentEventsPage';
import ProtectedRoute from './components/ProtectedRoute';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route
          path="/students"
          element={
            <ProtectedRoute>
              <StudentsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/students/:studentId/events"
          element={
            <ProtectedRoute>
              <StudentEventsPage />
            </ProtectedRoute>
          }
        />
        <Route path="/" element={<Navigate to="/students" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
