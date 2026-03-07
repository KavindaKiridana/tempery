import { useState, useEffect } from "react";

import {
  addDisplay,
  getDisplays,
  updateDisplay,
} from "../services/displayService";

import GenericAddMasterData from "./GenericAddMasterData";

import type { DisplayItem } from "@features/masterdata/types";

function AddDisplay() {
  const [displayName, setDisplayName] = useState("");
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [displays, setDisplays] = useState<DisplayItem[]>([]);

  // Fetch displays on component mount
  useEffect(() => {
    const fetchDisplays = async () => {
      const data = await getDisplays();
      setDisplays(data);
    };
    fetchDisplays();
  }, []);

  const handleSubmit = async () => {
    if (!displayName) {
      setSubmitError("Please enter a display name.");
      return;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    const DisplayData: DisplayItem = {
      Name: displayName,
    };
    const result = await addDisplay(DisplayData);
    setIsSubmitting(false);
    if (typeof result === "string") {
      setSubmitError(result);
    } else {
      setDisplayName("");
      // Refresh the list after adding a new display
      const data = await getDisplays();
      setDisplays(data);
    }
  };

  return (
    <GenericAddMasterData
      title="Add Display"
      label="Display Name"
      inputValue={displayName}
      onInputChange={setDisplayName}
      onSubmit={handleSubmit}
      isSubmitting={isSubmitting}
      error={submitError}
      items={displays}
      onUpdate={async (updatedItem) => {
        const result = await updateDisplay(updatedItem);
        if (result.success) {
          // Refresh the list after updating
          const data = await getDisplays();
          setDisplays(data);
        }
        return result;
      }}
    />
  );
}

export default AddDisplay;
