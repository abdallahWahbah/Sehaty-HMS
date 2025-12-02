export interface ApiResponse<T> {
  data: T;
  isSuccess: boolean;
  error: string | null;
  errorType: string;
}
