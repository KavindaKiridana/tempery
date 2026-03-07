import { useState, useEffect } from "react";

import { addOS, getOSs, updateOS } from "../services/osService";

import GenericAddMasterData from "./GenericAddMasterData";

import type { OSItem } from "@features/masterdata/types";

function AddOS() {
  const [osName, setOSName] = useState("");
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [osList, setOSList] = useState<OSItem[]>([]);

  // Fetch OS list on component mount
  useEffect(() => {
    const fetchOSs = async () => {
      const data = await getOSs();
      setOSList(data);
    };
    fetchOSs();
  }, []);

  const handleSubmit = async () => {
    if (!osName) {
      setSubmitError("Please enter an OS name.");
      return;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    const result = await addOS({ Name: osName });
    setIsSubmitting(false);
    if (typeof result === "string") {
      setSubmitError(result);
    } else {
      setOSName("");
      // Refresh the list after adding a new OS
      const data = await getOSs();
      setOSList(data);
    }
  };

  return (
    <GenericAddMasterData
      title="Add OS"
      label="OS Name"
      inputValue={osName}
      onInputChange={setOSName}
      onSubmit={handleSubmit}
      isSubmitting={isSubmitting}
      error={submitError}
      items={osList}
      onUpdate={async (updatedItem) => {
        const result = await updateOS(updatedItem);
        if (result.success) {
          // Refresh the list after updating
          const data = await getOSs();
          setOSList(data);
        }
        return result;
      }}
    />
  );
}

export default AddOS;
