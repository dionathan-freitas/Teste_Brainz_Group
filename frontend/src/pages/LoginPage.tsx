import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services/api';

export default function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const response = await authService.login({ username, password });
      authService.setToken(response.token, response.expiresAt);
      navigate('/students');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Falha no login. Verifique suas credenciais.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-500 via-purple-500 to-pink-500 relative overflow-hidden">
      <div className="absolute inset-0 bg-black/20"></div>
      <div className="absolute top-20 left-20 w-72 h-72 bg-white/10 rounded-full blur-3xl animate-pulse"></div>
      <div className="absolute bottom-20 right-20 w-96 h-96 bg-white/10 rounded-full blur-3xl animate-pulse" style={{ animationDelay: '1s' }}></div>
      
      <div className="card w-full max-w-md glass-effect shadow-2xl backdrop-blur-xl border border-white/20 relative z-10 elegant-btn">
        <div className="card-body">
          <div className="text-center mb-6">
            <div className="mb-4">
              <div className="w-20 h-20 mx-auto bg-gradient-to-br from-indigo-500 to-purple-600 rounded-full flex items-center justify-center shadow-lg">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-10 w-10 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
                </svg>
              </div>
            </div>
            <h2 className="text-4xl font-bold elegant-title mb-2">
              Student Events
            </h2>
            <p className="mt-2 text-sm text-base-content/80 font-medium">
              🎓 Faça login para acessar o sistema
            </p>
          </div>
          
          <form className="space-y-4" onSubmit={handleSubmit}>
            {error && (
              <div className="alert alert-error">
                <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span>{error}</span>
              </div>
            )}
            
            <div className="form-control">
              <label htmlFor="username" className="label">
                <span className="label-text font-medium">Usuário</span>
              </label>
              <input
                id="username"
                name="username"
                type="text"
                required
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                className="input input-bordered w-full focus:input-primary"
                placeholder="Digite seu usuário"
              />
            </div>
            
            <div className="form-control">
              <label htmlFor="password" className="label">
                <span className="label-text font-medium">Senha</span>
              </label>
              <input
                id="password"
                name="password"
                type="password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="input input-bordered w-full focus:input-primary"
                placeholder="Digite sua senha"
              />
            </div>
            
            <div className="form-control mt-6">
              <button
                type="submit"
                disabled={loading}
                className="btn btn-primary w-full elegant-btn text-lg font-semibold"
              >
                {loading ? (
                  <>
                    <span className="loading loading-spinner loading-md"></span>
                    Entrando...
                  </>
                ) : (
                  <>
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1" />
                    </svg>
                    Entrar
                  </>
                )}
              </button>
            </div>
          </form>
          
          <div className="divider">Credenciais padrão</div>
          
          <div className="text-sm text-center text-base-content/70 space-y-1">
            <p>Usuário: <kbd className="kbd kbd-sm">admin</kbd></p>
            <p>Senha: <kbd className="kbd kbd-sm">admin123</kbd></p>
          </div>
        </div>
      </div>
    </div>
  );
}
