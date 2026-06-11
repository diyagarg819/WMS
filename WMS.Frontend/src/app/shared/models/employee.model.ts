export interface Employee {
  employeeId: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  gender: string;
  dob: string;
  doj: string;
  departmentId: number;
  departmentName?: string;
  roleId: number;
  roleName?: string;
  status: string;
  username?: string;
}

export interface EmployeeDto {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  gender: string;
  dob: string;
  doj: string;
  departmentId: number;
  roleId: number;
  status: string;
  username?: string;
}
