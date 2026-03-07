export interface ReturnedFromRepair {
  AssetName?: string;
  SupplierId?: number;
  SupplierName?: string;
  Cost?: number;
  IsTempAssigned: boolean;
}

export interface ItObservationTracker {
  HasActiveItObservations: boolean;
  ActiveItObservations: ItObservation[];
}

export interface ItObservation {
  ObservationId: number;
  ObservedByName: string;
  ObservationNote: string;
  ObservationTime: string;
}

export interface Complains {
  ComplainId: number;
  UserName: string;
  Note: string;
  CreatedAt: string;
}

export interface UserItem {
  Id: number;
  FullName: string;
  IsActive: boolean;
}

export interface RemoveSpareParts {
  AssetId?: string;
  AssetName?: string;
  LocationName?: string;
  currentSparePartsList: AvailableAssetItem[];
}

export interface MoveAssetToLocationItem {
  AssetName?: string;
  HasExistingUser?: boolean;
  ExistingLocationId: number;
  ExitingCompanyId: number;
  ExistingLocationName: string;
  ExistingCompanyName: string;
  NextLocations: NextLocationsItem[];
}

export interface NextLocationsItem {
  LocationId: number;
  LocationName: string;
}

export interface AvailableAssetItem {
  AssetId: string;
  HasExistingUser?: boolean;
  AssetName: string;
  LocationId: number;
  LocationName: string;
  UserId?: number;
  UserName?: string;
}

export interface PostTransaction {
  Type: string;
  //ASSET_LOCATION_CHANGED, ASSET_ASSIGNED_TO_USER,  ASSET_ASSIGNED_TO_ASSET, ASSET_RETURNED_FROM_USER, ASSET_RETURNED_FROM_ASSET, ASSET_DESTROYED, ASSET_LOST_OR_STOLEN, GIVEN_TO_REAPAIR, RETURNED_FROM_REPAIR,STILL_IN_REPAIR
  AssetId?: string;
  Time: string;
  //if PostTransaction.Type is ASSET_ASSIGNED_TO_ASSET or ASSET_RETURNED_FROM_ASSET this FromId becomes a string value
  FromId?: number;
  FromIdString?: string;
  //if PostTransaction.Type is ASSET_ASSIGNED_TO_ASSET or ASSET_RETURNED_FROM_ASSET this ToId becomes a string value
  ToId?: number;
  ToIdString?: string;
  RelatedAssetId?: string;
  RelatedAssetName?: string;
  Note: string;
  IsTempAssigned?: boolean;
  RepairCost?: number | null;
  UserId?: number;
  ComplainId?: number;
  ObservationId?: number;
}

export interface GetTransaction {
  Type: string; //name of the list program should return
  AssetId?: string;
  UserId?: number;
  ComplainId?: number;
}

export interface TransactionPageData {
  AssetName?: string;
  HasOngoingRepair?: boolean;
  IsActiveAsset?: boolean;
  HasExistingUser?: boolean;
  IsActiveSparePart?: boolean; // IsAssetActiveSparePart
  HaveActiveSpareParts?: boolean;
  FromId?: number;
  FromName?: string;
  AssociateAssetId?: string;
}

export interface AssignToAsset {
  AssetId: string;
  AssetName: string;
  LocationId: number;
  LocationName: string;
  CompanyId: number;
  CompanyName: string;
  AvailableAssets: AvailableAssetItem[];
}
