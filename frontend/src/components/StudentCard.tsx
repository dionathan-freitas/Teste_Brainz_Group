import type { Student } from '../types';

interface StudentCardProps {
  student: Student;
  onViewEvents: (studentId: string) => void;
}

export default function StudentCard({ student, onViewEvents }: StudentCardProps) {
  return (
    <div className="elegant-card border-l-4 border-primary transform transition-all duration-300 hover:scale-105" role="listitem" aria-label={`Card estudante ${student.displayName}`}>        
      <div className="card-body">
        <div className="flex items-start gap-3">
          <div className="avatar placeholder">
            <div className="bg-gradient-to-br from-primary to-secondary text-neutral-content rounded-full w-12">
              <span className="text-lg font-bold">{student.displayName.charAt(0)}</span>
            </div>
          </div>
          <div className="flex-1">
            <h3 className="card-title text-lg font-bold text-transparent bg-clip-text bg-gradient-to-r from-primary to-secondary">
              {student.displayName}
            </h3>
            <div className="space-y-2 text-sm mt-2">
              <p className="text-base-content/70 flex items-center gap-1">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                </svg>
                {student.email}
              </p>
              {student.department && (
                <div className="badge badge-accent badge-sm gap-1">
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-3 w-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                  </svg>
                  {student.department}
                </div>
              )}
            </div>
          </div>
        </div>
        <div className="card-actions justify-end mt-4">
          <button 
            onClick={() => onViewEvents(student.id)}
            className="btn btn-primary btn-sm elegant-btn gap-2"
            aria-label={`Ver eventos de ${student.displayName}`}
          >
            <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            Ver eventos
          </button>
        </div>
      </div>
    </div>
  );
}
