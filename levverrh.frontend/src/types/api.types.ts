// API Response wrapper (espelha ResultDTO<T> do backend)
export interface ApiResponse<T = unknown> {
  success: boolean;
  message: string;
  data: T | null;
  errors: string[] | null;
}

// Pagination
export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// Error response
export interface ErrorResponse {
  message: string;
  errors?: Record<string, string[]>;
}
