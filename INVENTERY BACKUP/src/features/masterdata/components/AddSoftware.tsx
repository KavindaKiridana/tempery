import { useState, useEffect } from "react";

import {
  addSoftware,
  getSoftwares,
  updateSoftware,
} from "../services/softwareService";

import GenericAddMasterData from "./GenericAddMasterData";

function AddSoftware() {
  const [softwareName, setSoftwareName] = useState("");
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [softwareList, setSoftwareList] = useState<{ Name: string }[]>([]);

  // Fetch software list on component mount
  useEffect(() => {
    const fetchSoftwares = async () => {
      const data = await getSoftwares();
      setSoftwareList(data);
    };
    fetchSoftwares();
  }, []);

  const handleSubmit = async () => {
    if (!softwareName) {
      setSubmitError("Please enter a software name.");
      return;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    const result = await addSoftware({ Name: softwareName });
    setIsSubmitting(false);
    if (typeof result === "string") {
      setSubmitError(result);
    } else {
      setSoftwareName("");
      // Refresh the list after adding new software
      const data = await getSoftwares();
      setSoftwareList(data);
    }
  };

  return (
    <GenericAddMasterData
      title="Add Software"
      label="Software Name"
      inputValue={softwareName}
      onInputChange={setSoftwareName}
      onSubmit={handleSubmit}
      isSubmitting={isSubmitting}
      error={submitError}
      items={softwareList}
      onUpdate={async (updatedItem) => {
        const result = await updateSoftware(updatedItem);
        if (result.success) {
          // Refresh the list after updating
          const data = await getSoftwares();
          setSoftwareList(data);
        }
        return result;
      }}
    />
  );
}

export default AddSoftware;
