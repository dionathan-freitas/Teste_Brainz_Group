export interface Student {
  id: string;
  displayName: string;
  email: string;
  department: string | null;
  lastSyncDate: string;
}

export interface Event {
  id: string;
  subject: string;
  startDateTime: string;
  endDateTime: string;
  location: string | null;
  isOnlineMeeting: boolean;
}

export interface PaginatedResult<T> {
  data: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
}
