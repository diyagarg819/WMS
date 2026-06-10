export interface Department {
  departmentId: number;
  departmentName: string;
  description?: string;
  createdOn: string;
}

export interface CreateDepartmentRequest {
  departmentName: string;
  description?: string;
}

export interface UpdateDepartmentRequest extends CreateDepartmentRequest {
}
