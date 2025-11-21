declare const describe: any;
declare const it: any;
declare const expect: any;
declare const beforeEach: any;
const vi: any = (globalThis as any).vi ?? { fn: () => {}, mock: () => {} };

import { render, screen, waitFor } from '@testing-library/react';
import { Route, Routes } from 'react-router';
import { MemoryRouter } from 'react-router';

vi.mock('../services/api', () => {
  return {
    studentService: {
      getStudentEvents: vi.fn(async (studentId: string) => {
        return {
          student: { id: studentId, displayName: 'Alice Silva', email: 'alice@example.com', department: 'Tech' },
          events: [
            { id: 'e1', subject: 'Introdução ao C#', startDateTime: new Date().toISOString(), endDateTime: new Date().toISOString(), location: 'Sala 1', isOnlineMeeting: false },
          ],
        };
      }),
    },
    authService: {
      logout: vi.fn(),
    },
  };
});

import StudentEventsPage from '../pages/StudentEventsPage';

describe('StudentEventsPage', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('renders student info and events', async () => {
    render(
      <MemoryRouter initialEntries={["/students/1/events"]}>
        <Routes>
          <Route path="/students/:studentId/events" element={<StudentEventsPage />} />
        </Routes>
      </MemoryRouter>
    );
    await waitFor(() => {
      expect(screen.getByText(/Alice Silva/)).toBeInTheDocument();
      expect(screen.getByText(/Introdução ao C#/)).toBeInTheDocument();
      expect(screen.getByText(/1 evento/)).toBeInTheDocument();
    });
  });
});
