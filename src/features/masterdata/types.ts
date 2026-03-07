export interface ApiResponseBase {
  isSuccess: boolean;
  message: string;
}

export interface UsersList {
  Id: number;
  FullName: string;
  Email: string;
  Phone?: string;
  Designation?: string;
  AddedUserName?: string;
  AddedTime?: Date;
  DepartmentId?: number;
  DepartmentName?: string;
  LocationId?: number;
  LocationName?: string;
  isCapexUser: boolean;
  IsActive?: boolean;
  IsUsed?: boolean;
}

export interface GetUsersParams {
  needEveryUsers: boolean;
  requestedUserId?: number;
  search?: string;
}

export interface DepartmentItem {
  Id: number;
  Name: string;
  IsActive: boolean;
}

export interface LocationItem {
  Id: number;
  Name: string;
  IsActive: boolean;
  IsStockLocation: boolean;
}

export interface SupplierItem {
  Id: number;
  Name: string;
  Currency: string;
  IsActive: boolean;
}

export interface CompanyItem {
  Id: number;
  Name: string;
  IsActive: boolean;
}

export interface ProcessorItem {
  Id?: number;
  Name: string;
  IsActive?: boolean;
  IsUsed?: boolean;
}

export interface RAMSizeItem {
  Id?: number;
  Name: string;
  IsActive?: boolean;
  IsUsed?: boolean;
}

export interface RAMTypeItem {
  Id: number;
  Name: string;
  IsActive: boolean;
  IsUsed?: boolean;
}

export interface HDDItem {
  Id?: number;
  Name: string;
  IsActive?: boolean;
  IsUsed?: boolean;
}

export interface SSDItem {
  Id?: number;
  Name: string;
  IsActive?: boolean;
  IsUsed?: boolean;
}

export interface DisplayItem {
  Id?: number;
  Name: string;
  IsActive?: boolean;
  IsUsed?: boolean;
}

export interface ModelItem {
  Id?: number;
  Name: string;
  IsActive?: boolean;
  IsUsed?: boolean;
}

export interface OSItem {
  Id?: number;
  Name: string;
  IsActive?: boolean;
  IsUsed?: boolean;
}

export interface MasterData {
  departments: DepartmentItem[];
  locations: LocationItem[];
  suppliers: SupplierItem[];
  companies: CompanyItem[];
  processors: ProcessorItem[];
  ramSizes: RAMSizeItem[];
  ramTypes: RAMTypeItem[];
  hdds: HDDItem[];
  ssds: SSDItem[];
  displays: DisplayItem[];
  models: ModelItem[];
  oses: OSItem[];
}
