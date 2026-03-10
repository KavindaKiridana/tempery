import { useState, useEffect } from "react";

import {
  addRAMType,
  getRAMTypes,
} from "../services/ramTypeService";

import GenericAddMasterData from "./GenericAddMasterData";

function AddRAMType() {
  const [ramType, setRamType] = useState("");
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [ramTypeList, setRamTypeList] = useState<{ Name: string }[]>([]);

  // Fetch RAM type list on component mount
  useEffect(() => {
    const fetchRAMTypes = async () => {
      const data = await getRAMTypes();
      // Map Type to Name for generic component compatibility
      const formattedData = data.map((item) => ({ Name: item.Type, ...item }));
      setRamTypeList(formattedData);
    };
    fetchRAMTypes();
  }, []);

  const handleSubmit = async () => {
    if (!ramType) {
      setSubmitError("Please enter a RAM Type.");
      return;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    const result = await addRAMType({ Type: ramType });
    setIsSubmitting(false);
    if (typeof result === "string") {
      setSubmitError(result);
    } else {
      setRamType("");
      // Refresh the list after adding a new RAM Type
      const data = await getRAMTypes();
      const formattedData = data.map((item) => ({ Name: item.Type, ...item }));
      setRamTypeList(formattedData);
    }
  };

  return (
    <GenericAddMasterData
      title="Add RAM Type"
      label="RAM Type"
      inputValue={ramType}
      onInputChange={setRamType}
      onSubmit={handleSubmit}
      isSubmitting={isSubmitting}
      error={submitError}
      items={ramTypeList}
    />
  );
}

export default AddRAMType;
