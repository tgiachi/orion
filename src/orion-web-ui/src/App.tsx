import { observer } from "mobx-react-lite"
import { Route, Routes } from "react-router-dom"
import { IndexPage } from "./pages/IndexPage"
import { LoginPage } from "./pages/LoginPage"


const App = observer(() => {




  return (
    <Routes>
      <Route element={<IndexPage />} path="/" />
      <Route element={<LoginPage />} path="/login" />
    </Routes>
  )
})




export default App
