export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface PagedData<T> {
  data: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface PagedResponse<T> {
  success: boolean;
  message: string;
  data: PagedData<T>;
}
