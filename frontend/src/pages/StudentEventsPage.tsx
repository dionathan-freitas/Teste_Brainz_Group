import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { studentService, authService } from '../services/api';
import type { Student, Event } from '../types';

export default function StudentEventsPage() {
  const { studentId } = useParams();
  const [student, setStudent] = useState<Student | null>(null);
  const [events, setEvents] = useState<Event[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    if (studentId) {
      loadStudentEvents();
    }
  }, [studentId]);

  const loadStudentEvents = async () => {
    setLoading(true);
    setError('');
    try {
      const result = await studentService.getStudentEvents(studentId!);
      setStudent(result.student);
      setEvents(result.events);
    } catch (err: any) {
      if (err.response?.status === 401) {
        authService.logout();
        navigate('/login');
      } else {
        setError('Erro ao carregar eventos');
      }
    } finally {
      setLoading(false);
    }
  };

  const formatDateTime = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const formatCount = (n: number) => n.toLocaleString();

  return (
    <div className="min-h-screen bg-gradient-to-br from-base-200 via-base-100 to-base-200">
      <div className="navbar glass-effect shadow-2xl sticky top-0 z-50">
        <div className="flex-1">
          <button 
            type="button" 
            aria-label="Voltar para a lista de estudantes" 
            onClick={() => navigate('/students')} 
            className="btn btn-ghost btn-sm elegant-btn"
          >
            <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
            </svg>
            Voltar
          </button>
          <h1 className="text-xl elegant-title ml-4">📅 Eventos do Estudante</h1>
        </div>
      </div>

      <main className="container mx-auto px-4 py-8 max-w-7xl">
        {loading ? (
          <div className="flex flex-col items-center justify-center py-12">
            <span className="loading loading-spinner loading-lg text-primary"></span>
            <p className="mt-4 text-base-content/70">Carregando...</p>
          </div>
        ) : error ? (
          <div className="alert alert-error shadow-lg">
            <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <span>{error}</span>
          </div>
        ) : (
          <>
            {student && (
              <div className="elegant-card mb-6 border-t-4 border-primary">
                <div className="card-body">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center gap-3 mb-3">
                        <div className="avatar placeholder">
                          <div className="bg-gradient-to-br from-primary to-secondary text-neutral-content rounded-full w-16">
                            <span className="text-2xl font-bold">{student.displayName.charAt(0)}</span>
                          </div>
                        </div>
                        <div>
                          <h2 className="text-3xl font-bold elegant-title mb-1">{student.displayName}</h2>
                          <p className="text-base-content/70 flex items-center gap-2">
                            <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                            </svg>
                            {student.email}
                          </p>
                        </div>
                      </div>
                      {student.department && (
                        <div className="badge badge-accent badge-lg gap-2">
                          <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                          </svg>
                          {student.department}
                        </div>
                      )}
                    </div>
                    <div className="badge badge-primary badge-lg elegant-btn px-6 py-4">
                      <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                      </svg>
                      {formatCount(events.length)} evento{events.length !== 1 ? 's' : ''}
                    </div>
                  </div>
                </div>
              </div>
            )}

            {events.length === 0 ? (
              <div className="card bg-base-100 shadow-xl">
                <div className="card-body items-center text-center" role="status" aria-live="polite">
                  <p className="text-base-content/60">Nenhum evento encontrado para este estudante</p>
                </div>
              </div>
            ) : (
              <div className="elegant-card border-l-4 border-secondary">
                <div className="card-body">
                  <h3 className="card-title text-2xl mb-6 elegant-title">
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-7 w-7 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" />
                    </svg>
                    Lista de Eventos
                  </h3>
                  <div className="overflow-x-auto">
                    <table className="table table-zebra" role="list" aria-label="Lista de eventos">
                      <thead>
                        <tr>
                          <th>Evento</th>
                          <th>Início</th>
                          <th>Fim</th>
                          <th>Local</th>
                          <th>Tipo</th>
                        </tr>
                      </thead>
                      <tbody>
                        {events.map((event) => (
                          <tr key={event.id} className="hover" role="listitem">
                            <td>
                              <div className="font-semibold">{event.subject}</div>
                            </td>
                            <td className="text-sm">{formatDateTime(event.startDateTime)}</td>
                            <td className="text-sm">{formatDateTime(event.endDateTime)}</td>
                            <td className="text-sm">
                              {event.location ? (
                                <span>{event.location}</span>
                              ) : (
                                <span className="text-base-content/50">-</span>
                              )}
                            </td>
                            <td>
                              {event.isOnlineMeeting ? (
                                <div className="badge badge-info badge-sm">Online</div>
                              ) : (
                                <div className="badge badge-ghost badge-sm">Presencial</div>
                              )}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            )}
          </>
        )}
      </main>
    </div>
  );
}
