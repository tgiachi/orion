import { observer } from "mobx-react-lite";
import DefaultLayout from "../layouts/DefaultLayout";

export const IndexPage = observer(() => {
  return <DefaultLayout>
    <span>Index</span>
  </DefaultLayout>
})
