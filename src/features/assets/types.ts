import type {
  CompanyItem,
  DisplayItem,
  HDDItem,
  LocationItem,
  ModelItem,
  OSItem,
  ProcessorItem,
  RAMSizeItem,
  RAMTypeItem,
  SSDItem,
  SupplierItem,
} from "@features/masterdata/types";

// ========== Asset CRUD ==========
export interface CreateCommonAssetRequest {
  patchRequestType?: string;
  AssetId?: string;
  Type: string;
  Quantity: number;
  DoP: string;
  FinanceAssetCode: string;
  Warranty: number;
  CompanyId: number;
  LocationId: number;
  ManufactureSN: string;
  Brandnew: boolean;
  Cost: number;
  Name: string;
  SupplierId: number;
  DepartmentId?: number;
  IPAddress: string;
  Note: string;
}

export interface CreateAssetRequest {
  patchRequestType?: string;
  AssetId?: string;
  Type?: string;
  OsId: number;
  PId: number;
  RAMSId: number;
  RAMTId: number;
  HDDId: number;
  SSDId: number;
  Make: string;
  WindowsKey: string;
  Motherboard: string;
  ModelId: number;
  DisplayId?: number;
  PowerSupply?: boolean;
  RAIDSupport?: boolean;
}

export interface CreateAssetResponse {
  AssetIds: string[];
  Count: number;
}

export interface ReturnAssetId {
  AssetId: string;
}

export interface AssetView {
  AssetId: string;
  CompanyName: string | null;
  LocationName: string | null;
  SupplierName: string | null;
  DepartmentName: string | null;
  Type: string;
  Name: string | null;
  Cost: number | null;
  FinanceAssetCode: string | null;
  Warranty: number | null;
  ManufactureSN: string | null;
  CurrentUser: string | null;
  Brandnew: boolean | null;
  IPAddress: string | null;
  Remarks: string | null;
  Make: string | null;
  Model: string | null;
  IsActive: boolean;
  IsAvailable: boolean;
}

// ========== Asset Edit ==========
export interface EditAssetView {
  AssetId: string;
  Type: string;
  DoP: string | null;
  FinanceAssetCode: string | null;
  Warranty: number | null;
  CompanyId: number | null;
  CName: string | null;
  CompanyList: CompanyItem[];
  LocationId: number | null;
  LName: string | null;
  LocationList: LocationItem[];
  ManufactureSN: string | null;
  Brandnew: boolean | null;
  Cost: number | null;
  Name: string | null;
  SupplierId: number | null;
  SupplierName: string | null;
  SupplierList: SupplierItem[];
  IPAddress: string | null;
  Note: string | null;
  OsId: number | null;
  OS: string | null;
  OSList: OSItem[];
  PId: number | null;
  Processor: string | null;
  ProcessorList: ProcessorItem[];
  RAMSId: number | null;
  RAMSize: string | null;
  RAMSizeList: RAMSizeItem[];
  RAMTId: number | null;
  RAMType: string | null;
  RAMTypeList: RAMTypeItem[];
  HDDId: number | null;
  HDD: string | null;
  HDDList: HDDItem[];
  SSDId: number | null;
  SSD: string | null;
  SSDList: SSDItem[];
  DisplayId: number | null;
  Display: string | null;
  DisplayList: DisplayItem[];
  Make: string | null;
  WindowsKey: string | null;
  Motherboard: string | null;
  ModelId: number | null;
  Model: string | null;
  ModelList: ModelItem[];
  PowerSupply: boolean | null;
  RAIDSupport: boolean | null;
  InstalledSoftwares: InstalledSoftwareItem[] | null;
}

export interface InstalledSoftwareItem {
  SoftwareId: number;
  SoftwareName: string;
  IsActive: boolean;
}

// ========== Asset History ==========
export interface TransactionResponse {
  Type: string;
  Time: string;
  EditedUser: number;
  EditedUserFullName: string;
  FromId: number | null;
  FromName: string | null;
  ToId: number | null;
  ToName: string | null;
  RelatedAssetId?: string;
  RelatedAssetName?: string;
  Note: string | null;
}
