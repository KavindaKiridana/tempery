import { useEffect, useState } from "react";

import { addHDD, getHDDs, updateHDD } from "../services/hddService";

import GenericAddMasterData from "./GenericAddMasterData";

import type { HDDItem } from "@features/masterdata/types";

function AddHDD() {
  const [hddName, setHDDName] = useState("");
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [hdds, setHDDs] = useState<HDDItem[]>([]);

  // Fetch HDDs on component mount
  useEffect(() => {
    const fetchHDDs = async () => {
      const data = await getHDDs();
      setHDDs(data);
    };
    fetchHDDs();
  }, []);

  const handleSubmit = async () => {
    if (!hddName) {
      setSubmitError("Please enter a HDD name.");
      return;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    const HDDData: HDDItem = {
      Name: hddName,
    };
    const result = await addHDD(HDDData);
    setIsSubmitting(false);
    if (typeof result === "string") {
      setSubmitError(result);
    } else {
      setHDDName("");
      const data = await getHDDs();
      setHDDs(data);
    }
  };

  return (
    <GenericAddMasterData
      title="Add HDD"
      label="HDD Name"
      inputValue={hddName}
      onInputChange={setHDDName}
      onSubmit={handleSubmit}
      isSubmitting={isSubmitting}
      error={submitError}
      items={hdds}
      onUpdate={async (updatedItem) => {
        const result = await updateHDD(updatedItem);
        if (result.success) {
          // Refresh the list after updating
          const data = await getHDDs();
          setHDDs(data);
        }
        return result;
      }}
    />
  );
}
export default AddHDD;
