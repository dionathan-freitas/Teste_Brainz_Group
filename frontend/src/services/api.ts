import axios from 'axios';
import type { LoginRequest, LoginResponse, PaginatedResult, Student, Event } from '../types';

const API_BASE_URL = 'http://localhost:5035/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const { data } = await axios.post(`${API_BASE_URL}/auth/login`, credentials);
    // Normalize backend response: support `token` or `accessToken`, and `expiresAt` or `expiresAtUtc`
    const token = data.token ?? data.accessToken;
    const expiresAt = data.expiresAt ?? data.expiresAtUtc;
    return {
      token,
      expiresAt,
      username: data.username,
      role: data.role,
    } as LoginResponse;
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('tokenExpiry');
  },

  isAuthenticated: (): boolean => {
    const token = localStorage.getItem('token');
    const expiry = localStorage.getItem('tokenExpiry');
    if (!token || !expiry) return false;
    return new Date(expiry) > new Date();
  },

  setToken: (token: string, expiresAt: string) => {
    localStorage.setItem('token', token);
    localStorage.setItem('tokenExpiry', expiresAt);
  },
};

export const studentService = {
  getStudents: async (page: number, pageSize: number, search?: string, department?: string): Promise<PaginatedResult<Student>> => {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
    });
    if (search) params.append('search', search);
    if (department) params.append('department', department);
    
    const { data } = await api.get<PaginatedResult<Student>>(`/students?${params}`);
    return data;
  },

  getStudentEvents: async (studentId: string): Promise<{ student: Student; events: Event[] }> => {
    const { data } = await api.get<{ student: Student; events: Event[] }>(`/students/${studentId}/events`);
    return data;
  },
};

export const eventService = {
  getEvents: async (
    page: number,
    pageSize: number,
    studentId?: string,
    start?: string,
    end?: string,
    search?: string
  ): Promise<PaginatedResult<Event>> => {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
    });
    if (studentId) params.append('studentId', studentId);
    if (start) params.append('start', start);
    if (end) params.append('end', end);
    if (search) params.append('search', search);

    const { data } = await api.get<PaginatedResult<Event>>(`/events?${params}`);
    return data;
  },
};
