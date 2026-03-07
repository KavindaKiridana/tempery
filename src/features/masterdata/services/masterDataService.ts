import { fetchWithAuth } from "../../../utils/apiClient";

import type {
  CompanyItem,
  DepartmentItem,
  LocationItem,
  SupplierItem,
  ProcessorItem,
  RAMSizeItem,
  RAMTypeItem,
  HDDItem,
  SSDItem,
  DisplayItem,
  ModelItem,
  OSItem,
} from "../types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export const fetchModels = async (): Promise<ModelItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/Model`);
  if (!res.ok) throw new Error("Failed to load models");
  return res.json();
};

export const fetchDepartments = async (): Promise<DepartmentItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/Department`);
  if (!res.ok) throw new Error("Failed to load departments");
  return res.json();
};

export const fetchLocations = async (): Promise<LocationItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/Location`);
  if (!res.ok) throw new Error("Failed to load locations");
  return res.json();
};

export const fetchSuppliers = async (): Promise<SupplierItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/Supplier`);
  if (!res.ok) throw new Error("Failed to load suppliers");
  return res.json();
};

export const fetchCompanies = async (): Promise<CompanyItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/Company`);
  if (!res.ok) throw new Error("Failed to load companies");
  return res.json();
};

export const fetchProcessors = async (): Promise<ProcessorItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/Processor`);
  if (!res.ok) throw new Error("Failed to load processors");
  return res.json();
};

export const fetchRAMSizes = async (): Promise<RAMSizeItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/RAMSize`);
  if (!res.ok) throw new Error("Failed to load RAM sizes");
  return res.json();
};

export const fetchRAMTypes = async (): Promise<RAMTypeItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/RAMType`);
  if (!res.ok) throw new Error("Failed to load RAM types");
  return res.json();
};

export const fetchHDDs = async (): Promise<HDDItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/HDD`);
  if (!res.ok) throw new Error("Failed to load HDDs");
  return res.json();
};

export const fetchSSDs = async (): Promise<SSDItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/SSD`);
  if (!res.ok) throw new Error("Failed to load SSDs");
  return res.json();
};

export const fetchDisplays = async (): Promise<DisplayItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/Display`);
  if (!res.ok) throw new Error("Failed to load displays");
  return res.json();
};

export const fetchOSes = async (): Promise<OSItem[]> => {
  const res = await fetchWithAuth(`${API_BASE}/api/OS`);
  if (!res.ok) throw new Error("Failed to load OSes");
  return res.json();
};
