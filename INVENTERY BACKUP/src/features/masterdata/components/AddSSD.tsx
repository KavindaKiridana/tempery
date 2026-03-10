import { useState, useEffect } from "react";

import { addSSD, getSSDs, updateSSD } from "../services/ssdService";

import GenericAddMasterData from "./GenericAddMasterData";

import type { SSDItem } from "@features/masterdata/types";

function AddSSD() {
  const [ssdName, setSSDName] = useState("");
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [ssdList, setSSDList] = useState<SSDItem[]>([]);

  // Fetch SSD list on component mount
  useEffect(() => {
    const fetchSSDs = async () => {
      const data = await getSSDs();
      setSSDList(data);
    };
    fetchSSDs();
  }, []);

  const handleSubmit = async () => {
    if (!ssdName) {
      setSubmitError("Please enter an SSD name.");
      return;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    const result = await addSSD({ Name: ssdName });
    setIsSubmitting(false);
    if (typeof result === "string") {
      setSubmitError(result);
    } else {
      setSSDName("");
      // Refresh the list after adding a new SSD
      const data = await getSSDs();
      setSSDList(data);
    }
  };

  return (
    <GenericAddMasterData
      title="Add SSD"
      label="SSD Name"
      inputValue={ssdName}
      onInputChange={setSSDName}
      onSubmit={handleSubmit}
      isSubmitting={isSubmitting}
      error={submitError}
      items={ssdList}
      onUpdate={async (updatedItem) => {
        const result = await updateSSD(updatedItem);
        if (result.success) {
          // Refresh the list after updating
          const data = await getSSDs();
          setSSDList(data);
        }
        return result;
      }}
    />
  );
}

export default AddSSD;
