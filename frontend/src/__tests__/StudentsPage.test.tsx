import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router';
import '@testing-library/jest-dom/vitest';

vi.mock('../services/api', () => {
  return {
    studentService: {
      getStudents: vi.fn(async (page: number, pageSize: number) => {
        return {
          data: [
            { id: '1', displayName: 'Alice Silva', email: 'alice@example.com', department: 'Tech' },
            { id: '2', displayName: 'Bruno Costa', email: 'bruno@example.com', department: 'Biz' },
          ],
          totalPages: 1,
          totalCount: 2,
        };
      }),
    },
    authService: {
      logout: vi.fn(),
    },
  };
});

import StudentsPage from '../pages/StudentsPage';

describe('StudentsPage', () => {
  beforeEach(() => {
    // clear localStorage to avoid auth interference
    localStorage.clear();
  });

  it('renders students and total count', async () => {
    render(
      <MemoryRouter>
        <StudentsPage />
      </MemoryRouter>
    );

    await waitFor(() => {
      expect(screen.getAllByText(/Alice Silva/).length).toBeGreaterThan(0);
      expect(screen.getAllByText(/Bruno Costa/).length).toBeGreaterThan(0);
      expect(screen.getAllByText(/estudante/)[0]).toBeInTheDocument();
    });
  });
});
