import { useEffect, useState } from "react";

import {
  fetchDepartments,
  fetchLocations,
  fetchSuppliers,
  fetchCompanies,
  fetchProcessors,
  fetchRAMSizes,
  fetchRAMTypes,
  fetchHDDs,
  fetchSSDs,
  fetchDisplays,
  fetchModels,
  fetchOSes,
} from "@features/masterdata/services/masterDataService";

import type { MasterData } from "@features/masterdata/types";

export const useMasterData = () => {
  const [data, setData] = useState<MasterData>({
    departments: [],
    locations: [],
    suppliers: [],
    companies: [],
    processors: [],
    ramSizes: [],
    ramTypes: [],
    hdds: [],
    ssds: [],
    displays: [],
    models: [],
    oses: [],
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        const [
          departments,
          locations,
          suppliers,
          companies,
          processors,
          ramSizes,
          ramTypes,
          hdds,
          ssds,
          displays,
          models,
          oses,
        ] = await Promise.all([
          fetchDepartments(),
          fetchLocations(),
          fetchSuppliers(),
          fetchCompanies(),
          fetchProcessors(),
          fetchRAMSizes(),
          fetchRAMTypes(),
          fetchHDDs(),
          fetchSSDs(),
          fetchDisplays(),
          fetchModels(),
          fetchOSes(),
        ]);
        setData({
          departments,
          locations,
          suppliers,
          companies,
          processors,
          ramSizes,
          ramTypes,
          hdds,
          ssds,
          displays,
          models,
          oses,
        });
      } catch (err: any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    loadData();
  }, []);
  return { data, loading, error };
};
