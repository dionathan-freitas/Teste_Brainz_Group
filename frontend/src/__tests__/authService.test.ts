import { describe, it, expect, beforeEach } from 'vitest';
import { authService } from '../services/api';

describe('authService', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('stores token and expiry with setToken and reports authenticated', () => {
    const token = 'abc.xyz.def';
    const future = new Date(Date.now() + 1000 * 60 * 60).toISOString();
    authService.setToken(token, future);

    expect(localStorage.getItem('token')).toBe(token);
    expect(localStorage.getItem('tokenExpiry')).toBe(future);
    expect(authService.isAuthenticated()).toBe(true);
  });

  it('logout clears token and expiry and isAuthenticated returns false', () => {
    localStorage.setItem('token', 'x');
    localStorage.setItem('tokenExpiry', new Date(Date.now() + 1000 * 60).toISOString());

    authService.logout();

    expect(localStorage.getItem('token')).toBeNull();
    expect(localStorage.getItem('tokenExpiry')).toBeNull();
    expect(authService.isAuthenticated()).toBe(false);
  });

  it('isAuthenticated returns false with expired token', () => {
    localStorage.setItem('token', 'x');
    localStorage.setItem('tokenExpiry', new Date(Date.now() - 1000 * 60).toISOString());

    expect(authService.isAuthenticated()).toBe(false);
  });
});
