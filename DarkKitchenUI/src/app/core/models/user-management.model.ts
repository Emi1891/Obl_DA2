export type UserRole = 'Admin' | 'Client' | 'Dispatcher';

export interface UserListItem {
  id: number;
  name: string;
  surname: string;
  email: string;
  phone: string;
  role: UserRole;
}

export interface CreateUserPayload {
  name: string;
  surname: string;
  email: string;
  phone: string;
  password: string;
  role: UserRole;
}

export interface UpdateUserPayload {
  name?: string;
  surname?: string;
  email: string;
  phone?: string;
  password?: string;
}

export interface UserFilters {
  name?: string;
  surname?: string;
}
