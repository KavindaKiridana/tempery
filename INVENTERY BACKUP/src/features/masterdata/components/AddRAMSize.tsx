import { useEffect, useState } from "react";

import {
  addRAMSize,
  getRAMSizes,
  updateRAMSize,
} from "../services/ramSizeService";

import GenericAddMasterData from "./GenericAddMasterData";

import type { RAMSizeItem } from "@features/masterdata/types";

function AddRAMSize() {
  const [ramSize, setRamSize] = useState("");
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [ramSizeList, setRamSizeList] = useState<{ Name: string }[]>([]);

  // Fetch RAM size list on component mount
  useEffect(() => {
    const fetchRAMSizes = async () => {
      const data = await getRAMSizes();
      setRamSizeList(data);
    };
    fetchRAMSizes();
  }, []);

  const handleSubmit = async () => {
    if (!ramSize) {
      setSubmitError("Please enter a RAM Size.");
      return;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    const RAMSizeData: RAMSizeItem = {
      Name: ramSize,
    };
    const result = await addRAMSize(RAMSizeData);
    setIsSubmitting(false);
    if (typeof result === "string") {
      setSubmitError(result);
    } else {
      setRamSize("");
      // Refresh the list after adding a new RAM Size
      const data = await getRAMSizes();
      setRamSizeList(data);
    }
  };

  return (
    <GenericAddMasterData
      title="Add RAM Size"
      label="RAM Size"
      inputValue={ramSize}
      onInputChange={setRamSize}
      onSubmit={handleSubmit}
      isSubmitting={isSubmitting}
      error={submitError}
      items={ramSizeList}
      onUpdate={async (updatedItem) => {
        const result = await updateRAMSize(updatedItem);
        if (result.success) {
          const data = await getRAMSizes();
          setRamSizeList(data);
        }
        return result;
      }}
    />
  );
}

export default AddRAMSize;
