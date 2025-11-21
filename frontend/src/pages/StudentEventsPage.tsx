import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { studentService, authService } from '../services/api';
import type { Student, Event } from '../types';

export default function StudentEventsPage() {
  const { studentId } = useParams<{ studentId: string }>();
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

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center gap-4">
          <button
            onClick={() => navigate('/students')}
            className="text-blue-600 hover:text-blue-800"
          >
            ← Voltar
          </button>
          <h1 className="text-2xl font-bold text-gray-900">Eventos do Estudante</h1>
        </div>
      </nav>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {loading ? (
          <div className="text-center py-12">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
            <p className="mt-2 text-gray-600">Carregando...</p>
          </div>
        ) : error ? (
          <div className="bg-red-50 border border-red-400 text-red-700 px-4 py-3 rounded">
            {error}
          </div>
        ) : (
          <>
            {student && (
              <div className="bg-white rounded-lg shadow p-6 mb-6">
                <h2 className="text-xl font-semibold mb-2">
                  {student.displayName}
                  <span className="text-sm text-gray-600 ml-3">({events.length} evento{events.length !== 1 ? 's' : ''})</span>
                </h2>
                <p className="text-gray-600">{student.email}</p>
                {student.department && (
                  <p className="text-sm text-gray-500 mt-1">Departamento: {student.department}</p>
                )}
              </div>
            )}

            {events.length === 0 ? (
              <div className="bg-white rounded-lg shadow p-12 text-center">
                <p className="text-gray-500">Nenhum evento encontrado para este estudante</p>
              </div>
            ) : (
              <div className="space-y-4">
                {events.map((event) => (
                  <div key={event.id} className="bg-white rounded-lg shadow p-6 hover:shadow-md transition-shadow">
                    <div className="flex items-start justify-between">
                      <div className="flex-1">
                        <h3 className="text-lg font-semibold text-gray-900 mb-2">
                          {event.subject}
                        </h3>
                        <div className="space-y-1 text-sm text-gray-600">
                          <p>
                            <span className="font-medium">Início:</span> {formatDateTime(event.startDateTime)}
                          </p>
                          <p>
                            <span className="font-medium">Fim:</span> {formatDateTime(event.endDateTime)}
                          </p>
                          {event.location && (
                            <p>
                              <span className="font-medium">Local:</span> {event.location}
                            </p>
                          )}
                        </div>
                      </div>
                      {event.isOnlineMeeting && (
                        <span className="ml-4 px-3 py-1 bg-blue-100 text-blue-800 text-xs font-medium rounded-full">
                          Online
                        </span>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </>
        )}
      </main>
    </div>
  );
}
