import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { studentService, authService } from '../services/api';
import StudentCard from '../components/StudentCard';
import type { Student } from '../types';

export default function StudentsPage() {
  const [students, setStudents] = useState<Student[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [search, setSearch] = useState('');
  const [department, setDepartment] = useState('');
  const [searchInput, setSearchInput] = useState('');
  const [deptInput, setDeptInput] = useState('');
  const debounceRef = useRef<number | null>(null);
  const navigate = useNavigate();

  const pageSize = 10;

  const formatCount = (n: number) => n.toLocaleString();

  useEffect(() => {
    loadStudents();
  }, [page, search, department]);

  // Debounce inputs for search/department for smoother UX
  useEffect(() => {
    if (debounceRef.current) {
      window.clearTimeout(debounceRef.current);
    }
    debounceRef.current = window.setTimeout(() => {
      setSearch(searchInput);
      setDepartment(deptInput);
      setPage(1);
    }, 400); // 400ms debounce
    return () => {
      if (debounceRef.current) window.clearTimeout(debounceRef.current);
    };
  }, [searchInput, deptInput]);

  const loadStudents = async () => {
    setLoading(true);
    setError('');
    try {
    const result = await studentService.getStudents(page, pageSize, search || undefined, department || undefined);
    setStudents(result.data);
    setTotalPages(result.totalPages);
    setTotalCount(result.totalCount ?? 0);
    } catch (err: any) {
      if (err.response?.status === 401) {
        authService.logout();
        navigate('/login');
      } else {
        setError('Erro ao carregar estudantes');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    // Immediate apply ignoring debounce
    if (debounceRef.current) window.clearTimeout(debounceRef.current);
    setSearch(searchInput);
    setDepartment(deptInput);
    setPage(1);
  };

  const handleClear = () => {
    setSearchInput('');
    setDeptInput('');
    setSearch('');
    setDepartment('');
    setPage(1);
  };

  const handleLogout = () => {
    authService.logout();
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-base-200 via-base-100 to-base-200">
      <a href="#conteudo" className="sr-only focus:not-sr-only focus:underline">Ir para conteúdo</a>
      <div className="navbar glass-effect shadow-2xl sticky top-0 z-50">
        <div className="flex-1">
          <a className="btn btn-ghost text-xl elegant-title">Student Events</a>
          <div className="badge badge-primary badge-lg ml-3 elegant-btn" aria-live="polite">
            {formatCount(totalCount)} estudante{totalCount !== 1 ? 's' : ''}
          </div>
        </div>
        <div className="flex-none">
          <button onClick={handleLogout} className="btn btn-ghost btn-sm elegant-btn hover:btn-error">
            <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
            Sair
          </button>
        </div>
      </div>

      <main id="conteudo" className="container mx-auto px-4 py-8 max-w-7xl" aria-busy={loading}>
        <div className="elegant-card mb-6 border-t-4 border-primary">
          <div className="card-body">
            <h2 className="card-title text-2xl mb-4 elegant-title">🔍 Filtros</h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div className="form-control">
                <label className="label">
                  <span className="label-text font-medium">Buscar por nome ou email</span>
                </label>
                <input
                  type="text"
                  value={searchInput}
                  onChange={(e) => setSearchInput(e.target.value)}
                  placeholder="Digite para buscar..."
                  className="input input-bordered w-full"
                />
              </div>
              <div className="form-control">
                <label className="label">
                  <span className="label-text font-medium">Departamento</span>
                </label>
                <input
                  type="text"
                  value={deptInput}
                  onChange={(e) => setDeptInput(e.target.value)}
                  placeholder="Ex: Tech, Biz..."
                  className="input input-bordered w-full"
                />
              </div>
              <div className="flex items-end gap-2">
                <button onClick={handleSearch} className="btn btn-primary flex-1 elegant-btn">
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                  </svg>
                  Filtrar
                </button>
                <button onClick={handleClear} className="btn btn-outline flex-1 elegant-btn hover:btn-secondary">
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                  Limpar
                </button>
              </div>
            </div>
          </div>
        </div>

        {error && (
          <div className="alert alert-error shadow-lg mb-6">
            <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <span>{error}</span>
          </div>
        )}

        {loading ? (
          <div className="elegant-card mb-6 border-l-4 border-info" aria-label="Carregando estudantes" role="status" aria-live="polite">
            <div className="card-body">
              <div className="flex items-center gap-4 mb-6">
                <div className="skeleton h-10 w-48"></div>
                <div className="skeleton h-10 w-24"></div>
              </div>
              {/* Skeleton table */}
              <div className="hidden md:block overflow-x-auto">
                <table className="table w-full" aria-hidden="true">
                  <thead>
                    <tr>
                      <th><div className="skeleton h-4 w-20"></div></th>
                      <th><div className="skeleton h-4 w-24"></div></th>
                      <th><div className="skeleton h-4 w-24"></div></th>
                      <th><div className="skeleton h-4 w-16"></div></th>
                    </tr>
                  </thead>
                  <tbody>
                    {Array.from({ length: pageSize }).map((_, i) => (
                      <tr key={i}>
                        <td><div className="skeleton h-4 w-36"></div></td>
                        <td><div className="skeleton h-4 w-40"></div></td>
                        <td><div className="skeleton h-4 w-28"></div></td>
                        <td><div className="skeleton h-8 w-24"></div></td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              {/* Mobile skeleton cards */}
              <div className="md:hidden grid grid-cols-1 gap-4" aria-hidden="true">
                {Array.from({ length: 4 }).map((_, i) => (
                  <div key={i} className="elegant-card p-4">
                    <div className="skeleton h-6 w-40 mb-2"></div>
                    <div className="skeleton h-4 w-48 mb-2"></div>
                    <div className="skeleton h-4 w-24 mb-4"></div>
                    <div className="skeleton h-8 w-32"></div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        ) : students.length === 0 ? (
          <div className="card bg-base-100 shadow-xl">
            <div className="card-body items-center text-center">
              <p className="text-base-content/60">Nenhum estudante encontrado</p>
            </div>
          </div>
        ) : (
          <>
            <div className="elegant-card mb-6 border-l-4 border-accent">
              <div className="card-body">
                <div className="flex items-center justify-between mb-4 pb-4 border-b-2 border-gray-100">
                  <div>
                    <div className="text-sm text-base-content/60">Total</div>
                    <div className="text-lg font-semibold">{formatCount(totalCount)} estudante{totalCount !== 1 ? 's' : ''}</div>
                  </div>
                  <div className="text-sm text-base-content/60">Página {page} de {totalPages}</div>
                </div>
                
                {/* Desktop: table */}
                <div className="hidden md:block overflow-x-auto">
                  <table className="table table-zebra w-full" role="table" aria-label="Lista de estudantes">
                    <thead>
                      <tr>
                        <th>Nome</th>
                        <th>Email</th>
                        <th>Departamento</th>
                        <th>Ações</th>
                      </tr>
                    </thead>
                    <tbody>
                      {students.map((student) => (
                        <tr key={student.id} className="hover" aria-label={`Estudante ${student.displayName}`}>
                          <td className="font-medium">{student.displayName}</td>
                          <td className="text-base-content/70">{student.email}</td>
                          <td>
                            {student.department ? (
                              <div className="badge badge-ghost">{student.department}</div>
                            ) : (
                              <span className="text-base-content/50">-</span>
                            )}
                          </td>
                          <td>
                            <button 
                              onClick={() => navigate(`/students/${student.id}/events`)} 
                              className="btn btn-primary btn-sm"
                              aria-label={`Ver eventos de ${student.displayName}`}
                            >
                              Ver eventos
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                {/* Mobile: cards */}
                <div className="md:hidden grid grid-cols-1 gap-4">
                  {students.map((student) => (
                    <StudentCard
                      key={student.id}
                      student={student}
                      onViewEvents={(id) => navigate(`/students/${id}/events`)}
                    />
                  ))}
                </div>
              </div>
            </div>

            {totalPages > 1 && (
              <div className="flex justify-center">
                <div className="join">
                  <button
                    onClick={() => setPage(p => Math.max(1, p - 1))}
                    disabled={page === 1}
                    className="join-item btn btn-outline"
                    aria-label="Página anterior"
                  >
                    « Anterior
                  </button>
                  <button className="join-item btn btn-outline">
                    Página {page} de {totalPages}
                  </button>
                  <button
                    onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                    disabled={page === totalPages}
                    className="join-item btn btn-outline"
                    aria-label="Próxima página"
                  >
                    Próxima »
                  </button>
                </div>
              </div>
            )}
          </>
        )}
      </main>
    </div>
  );
}
