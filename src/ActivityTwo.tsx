import { Button, CircularProgress } from "@mui/material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

function ActivityTwo() {
  const [showButton, setShowButton] = useState(true);
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const submitForm = async () => {
    setLoading(true);
    try {
      // simulate network delay and random failure
      await new Promise((res) => setTimeout(res, 1000));
      if (Math.random() < 0.5) throw new Error("Simulated Failure");
      setShowButton(false);
    } catch (error: any) {
      setError(error.message);
    } finally {
      setLoading(false);
    }
  };

  if (loading)
    return (
      <div
        style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "80vh",
        }}
      >
        <CircularProgress />
      </div>
    );
  if (error)
    return (
      <div
        style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "80vh",
        }}
      >
        {error}
      </div>
    );

  return (
    <>
      {showButton && (
        <div
          style={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            height: "80vh",
          }}
        >
          <Button
            variant="contained"
            size="large"
            sx={{ minWidth: 200 }}
            onClick={submitForm}
          >
            Click Me
          </Button>
        </div>
      )}
      <Button
        variant="contained"
        size="large"
        color="warning"
        sx={{ minWidth: 200 }}
        onClick={() => navigate("/")}
      >
        Go Back
      </Button>
    </>
  );
}
export default ActivityTwo;
