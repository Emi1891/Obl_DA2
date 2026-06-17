export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

export interface RegisterRequest {
  name: string;
  surname: string;
  email: string;
  phone?: string;
  password: string;
}
