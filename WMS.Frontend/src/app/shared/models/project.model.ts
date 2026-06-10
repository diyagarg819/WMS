

export interface ProjectAllocation {
  allocationId: number;
  empId: number;
  employeeName: string;
  projectId: number;
  projectName: string;
  assignedOn: string;
  status: boolean;
  createdBy: string;
}

export interface ProjectRecord {
  projectId: number;
  projectName: string;
  clientId?: number;
  startDate?: string;
  endDate?: string;
  status: string;
  allocations: ProjectAllocation[];
}

export interface CreateProjectRequest {
  projectName: string;
  clientId?: number;
  startDate?: string;
  endDate?: string;
  status: string;
}

export interface UpdateProjectRequest extends CreateProjectRequest {
}

export interface AssignEmployeeRequest {
  empId: number;
}

export interface UpdateAllocationStatusRequest {
  status: boolean;
}
